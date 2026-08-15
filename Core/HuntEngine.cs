using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using HuntAutomator.Data;
using HuntAutomator.Integrations;
using HuntAutomator.Models;
using HuntAutomator.Utils;
using Lumina.Excel.Sheets;

namespace HuntAutomator.Core;

public enum EngineState
{
    Idle, Planning, Teleporting, WaitingForZone, Navigating, Searching,
    Mounting, Patrolling, Fighting, Confirming, Recovering, Finished, Error
}

internal sealed class HuntEngine
{
    private readonly Configuration cfg;
    private readonly VNavmeshIpc nav = new();
    private readonly RotationSolverIpc rsr = new();
    private readonly TeleporterIpc tp = new();
    private readonly HuntHelperIpc hh = new();
    private readonly Queue<HuntTarget> queue = new();
    private readonly Queue<Vector3> patrol = new();

    private HuntTarget? current;
    private DateTime stateSince;
    private int retries;
    private int startingKillCount;
    private Vector3? currentPatrolPoint;
    private DateTime? zoneReadySince;

    public EngineState State { get; private set; } = EngineState.Idle;
    public string Status { get; private set; } = "Idle";
    public HuntTarget? Current => current;
    public int Remaining => queue.Count + (current is null ? 0 : 1);
    public int PatrolPointsRemaining => patrol.Count + (currentPatrolPoint is null ? 0 : 1);
    public bool NavReady => nav.IsReady();
    public bool RotationSolverReady => rsr.IsAvailable();

    public HuntEngine(Configuration cfg) => this.cfg = cfg;

    public void Start()
    {
        Stop();
        if (Service.ObjectTable.LocalPlayer is null)
        {
            State = EngineState.Error;
            Status = "Player is not logged in.";
            return;
        }
        if (!nav.IsReady())
        {
            State = EngineState.Error;
            Status = "vnavmesh is unavailable or its mesh is not ready.";
            return;
        }
        if (!rsr.IsAvailable())
        {
            State = EngineState.Error;
            Status = "RotationSolverReborn is unavailable (/rotation not registered).";
            return;
        }

        State = EngineState.Planning;
        Status = "Reading hunt bills and HuntHelper train";
        BuildQueue();
        NextTarget();
    }

    public void Reload()
    {
        if (State is EngineState.Idle or EngineState.Finished or EngineState.Error) Start();
        else
        {
            nav.Stop();
            if (current is not null) rsr.Stop(current.MobId);
            queue.Clear();
            patrol.Clear();
            current = null;
            BuildQueue();
            NextTarget();
        }
    }

    public void Stop()
    {
        if (current is not null) rsr.Stop(current.MobId);
        nav.Stop();
        queue.Clear();
        patrol.Clear();
        currentPatrolPoint = null;
        zoneReadySince = null;
        current = null;
        State = EngineState.Idle;
        Status = "Idle";
    }

    private void BuildQueue()
    {
        var targets = HuntBillReader.ReadCurrentBills(cfg)
            .OrderBy(x => x.TerritoryId)
            .ThenBy(x => x.Kind)
            .ThenBy(x => x.Name);
        foreach (var target in targets) queue.Enqueue(target);

        if (!cfg.RunARanks || !cfg.UseHuntHelperTrain) return;

        var train = hh.TryGetTrain().Where(x => !x.Dead).ToList();
        if (train.Count == 0)
        {
            Service.Log.Information("HuntHelper train IPC returned no usable marks; daily/weekly hunts will still run.");
            return;
        }

        foreach (var mark in train)
        {
            // A recorded HuntHelper train is intentionally trusted as the A-rank source. Entries marked dead are filtered above.
            queue.Enqueue(new HuntTarget(
                mark.MobId, mark.Name, mark.TerritoryId, mark.MapId, 0,
                HuntKind.ARank, 0, 0, 1, 0, mark.Position));
        }
    }

    private void NextTarget()
    {
        patrol.Clear();
        currentPatrolPoint = null;

        if (!queue.TryDequeue(out current))
        {
            State = EngineState.Finished;
            Status = "No eligible incomplete hunt bills or live HuntHelper marks found.";
            return;
        }

        retries = 0;
        startingKillCount = current.Kind == HuntKind.Daily ? HuntBillReader.GetKillCount(current) : 0;
        BeginTarget();
    }

    private void BeginTarget()
    {
        if (current is null) return;

        // If the daily bill already advanced between planning and execution, don't waste time.
        if (current.Kind == HuntKind.Daily && HuntBillReader.GetKillCount(current) >= current.NeededKills)
        {
            NextTarget();
            return;
        }

        if (Service.ClientState.TerritoryType != current.TerritoryId)
        {
            State = EngineState.Teleporting;
            Status = $"Teleporting for {current.Name}";
            stateSince = DateTime.UtcNow;
            zoneReadySince = null;
            if (!tp.TeleportToTerritory(current.TerritoryId))
            {
                FailOrRetry("No usable aetheryte / Teleporter IPC failed");
                return;
            }
            State = EngineState.WaitingForZone;
            stateSince = DateTime.UtcNow;
            return;
        }

        PrepareSearch();
    }

    private void PrepareSearch()
    {
        if (current is null) return;

        var live = TargetScanner.Find(current.MobId, cfg.SearchRadius);
        if (live is not null)
        {
            Engage(live);
            return;
        }

        var map = Service.DataManager.GetExcelSheet<Map>().GetRowOrDefault(current.MapId);
        if (map is null)
        {
            FailOrRetry("Map data missing");
            return;
        }

        patrol.Clear();
        currentPatrolPoint = null;

        if (current.MapPosition is { } known)
        {
            // First check the known bill/train region, then a small spiral around it.
            foreach (var p in BuildLocalMapPattern(known, cfg.PatrolMapStep))
                AddPatrolPoint(p, map.Value);
        }

        // Elite marks can be at many spawn locations. Daily coordinates can also be stale or crowded,
        // so every hunt gets a deterministic full-map fallback patrol.
        foreach (var p in BuildFullMapPattern(cfg.PatrolMapStep))
            AddPatrolPoint(p, map.Value);

        MoveNextPatrolPoint();
    }

    private void AddPatrolPoint(Vector2 mapPosition, Map map)
    {
        var world = MapCoordinates.MapToApproxWorld(mapPosition, map);
        var snapped = nav.SnapToMesh(world);
        if (snapped is null) return;

        // Avoid dozens of duplicate snaps around inaccessible map-coordinate points.
        if (patrol.Any(p => Vector3.DistanceSquared(p, snapped.Value) < 100f)) return;
        patrol.Enqueue(snapped.Value);
    }

    private static IEnumerable<Vector2> BuildLocalMapPattern(Vector2 center, float step)
    {
        yield return center;
        var d = Math.Clamp(step, 2.5f, 7f);
        for (var ring = 1; ring <= 2; ring++)
        {
            var r = d * ring;
            yield return center + new Vector2(r, 0);
            yield return center + new Vector2(0, r);
            yield return center + new Vector2(-r, 0);
            yield return center + new Vector2(0, -r);
            yield return center + new Vector2(r, r);
            yield return center + new Vector2(-r, r);
            yield return center + new Vector2(-r, -r);
            yield return center + new Vector2(r, -r);
        }
    }

    private static IEnumerable<Vector2> BuildFullMapPattern(float step)
    {
        var s = Math.Clamp(step, 3f, 7f);
        var row = 0;
        for (var y = 5f; y <= 37f; y += s, row++)
        {
            if ((row & 1) == 0)
                for (var x = 5f; x <= 37f; x += s) yield return new Vector2(x, y);
            else
                for (var x = 37f; x >= 5f; x -= s) yield return new Vector2(x, y);
        }
    }

    private void MoveNextPatrolPoint()
    {
        if (current is null) return;
        if (!patrol.TryDequeue(out var next))
        {
            FailOrRetry("Completed map patrol without finding target");
            return;
        }

        currentPatrolPoint = next;
        if (cfg.PreferFlying && !Service.Condition[ConditionFlag.Mounted])
        {
            State = EngineState.Mounting;
            Status = $"Mounting to fly to {current.Name}";
            stateSince = DateTime.UtcNow;
            if (!MountController.TryMount())
            {
                Service.Log.Warning("Mount Roulette could not be started for {Name}; using a ground route.", current.Name);
                StartPatrolMovement(false);
            }
            else
                Service.Log.Information("Mounting for flying route to {Name}.", current.Name);
            return;
        }

        StartPatrolMovement(cfg.PreferFlying);
    }

    private void StartPatrolMovement(bool fly)
    {
        if (current is null || currentPatrolPoint is not { } next) return;
        State = EngineState.Patrolling;
        Status = $"Patrolling for {current.Name} ({PatrolPointsRemaining} points remain){(fly ? " by air" : " on foot")}";
        stateSince = DateTime.UtcNow;
        if (!nav.MoveTo(next, fly, 12f)) MoveNextPatrolPoint();
    }

    public void Tick()
    {
        if (current is null || State is EngineState.Idle or EngineState.Finished or EngineState.Error) return;
        var now = DateTime.UtcNow;
        var player = Service.ObjectTable.LocalPlayer;

        if (player is null)
        {
            Fatal("Local player disappeared.");
            return;
        }

        if (cfg.StopOnPlayerDeath && player.CurrentHp == 0)
        {
            Fatal("Player died; automation stopped for manual recovery.");
            return;
        }

        if (State == EngineState.WaitingForZone)
        {
            if (Service.ClientState.TerritoryType == current.TerritoryId)
            {
                // The territory changes before its navmesh is necessarily loaded. Require
                // two continuous ready seconds before querying or starting movement.
                if (!nav.IsReady())
                {
                    zoneReadySince = null;
                    Status = $"Waiting for navmesh in {current.Name}'s zone";
                    if ((now - stateSince).TotalSeconds > 45)
                        FailOrRetry("Navmesh did not become ready after teleport");
                }
                else
                {
                    zoneReadySince ??= now;
                    Status = $"Waiting for {current.Name}'s zone to settle";
                    if ((now - zoneReadySince.Value).TotalSeconds >= 2)
                    {
                        zoneReadySince = null;
                        PrepareSearch();
                    }
                }
            }
            else if ((now - stateSince).TotalSeconds > 45)
                FailOrRetry("Zone load timeout");
            return;
        }

        if (State == EngineState.Mounting)
        {
            if (Service.Condition[ConditionFlag.Mounted])
                StartPatrolMovement(true);
            else if ((now - stateSince).TotalSeconds > 5)
            {
                Service.Log.Warning("Could not mount for {Name}; using a ground route.", current.Name);
                StartPatrolMovement(false);
            }
            return;
        }

        var obj = TargetScanner.Find(current.MobId, cfg.SearchRadius);
        if (obj is not null && State is EngineState.Patrolling or EngineState.Searching or EngineState.Navigating)
        {
            nav.Stop();
            Engage(obj);
            return;
        }

        if (State == EngineState.Patrolling)
        {
            if ((now - stateSince).TotalSeconds > cfg.NavigationTimeoutSeconds)
            {
                nav.Stop();
                MoveNextPatrolPoint();
            }
            else if (!nav.IsBusy())
            {
                MoveNextPatrolPoint();
            }
            return;
        }

        if (State == EngineState.Fighting)
        {
            // Daily bills have the best source of truth: MobHunt kill count.
            if (current.Kind == HuntKind.Daily && HuntBillReader.GetKillCount(current) > startingKillCount)
            {
                rsr.Stop(current.MobId);
                State = EngineState.Confirming;
                stateSince = now;
                return;
            }

            var target = Service.TargetManager.Target;
            if (target is null || !target.IsTargetable)
            {
                rsr.Stop(current.MobId);
                State = EngineState.Confirming;
                stateSince = now;
                return;
            }

            if ((now - stateSince).TotalSeconds > cfg.CombatTimeoutSeconds)
            {
                rsr.Stop(current.MobId);
                FailOrRetry("Combat timeout");
            }
            return;
        }

        if (State == EngineState.Confirming)
        {
            if ((now - stateSince).TotalMilliseconds < 750) return;

            if (current.Kind != HuntKind.Daily)
            {
                NextTarget();
                return;
            }

            var count = HuntBillReader.GetKillCount(current);
            if (count >= current.NeededKills)
            {
                NextTarget();
                return;
            }

            startingKillCount = count;
            PrepareSearch();
        }
    }

    private void Engage(Dalamud.Game.ClientState.Objects.Types.IGameObject obj)
    {
        if (current is null) return;
        nav.Stop();
        Service.TargetManager.Target = obj;
        if (!rsr.StartManual(current.MobId))
        {
            FailOrRetry("Could not enable RotationSolverReborn");
            return;
        }
        State = EngineState.Fighting;
        Status = $"Fighting {current.Name}";
        stateSince = DateTime.UtcNow;
    }

    private void FailOrRetry(string reason)
    {
        if (current is null) return;
        nav.Stop();
        rsr.Stop(current.MobId);
        patrol.Clear();
        currentPatrolPoint = null;
        retries++;

        if (retries <= cfg.MaxRetriesPerTarget)
        {
            Status = $"Retry {retries}/{cfg.MaxRetriesPerTarget}: {reason}";
            BeginTarget();
        }
        else
        {
            Service.Log.Warning("Skipping {Name}: {Reason}", current.Name, reason);
            NextTarget();
        }
    }

    private void Fatal(string reason)
    {
        if (current is not null) rsr.Stop(current.MobId);
        nav.Stop();
        State = EngineState.Error;
        Status = reason;
    }
}
