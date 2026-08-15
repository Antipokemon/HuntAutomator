using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace HuntAutomator.UI;

internal sealed class MainWindow : Window
{
    private readonly Plugin plugin;
    public MainWindow(Plugin p) : base("Hunt Automator###HuntAutomator") => plugin = p;

    public override void Draw()
    {
        ImGui.TextUnformatted($"State: {plugin.Engine.State}");
        ImGui.TextWrapped(plugin.Engine.Status);
        ImGui.TextUnformatted($"Queued: {plugin.Engine.Remaining}");
        if (plugin.Engine.PatrolPointsRemaining > 0)
            ImGui.TextUnformatted($"Patrol points: {plugin.Engine.PatrolPointsRemaining}");
        if (plugin.Engine.Current is { } t)
            ImGui.TextUnformatted($"Target: {t.Name} ({t.Kind})");

        ImGui.Separator();
        ImGui.TextUnformatted($"vnavmesh: {(plugin.Engine.NavReady ? "Ready" : "Unavailable")}");
        ImGui.TextUnformatted($"RotationSolverReborn: {(plugin.Engine.RotationSolverReady ? "Ready" : "Unavailable")}");
        ImGui.Separator();

        if (ImGui.Button("Start")) plugin.Engine.Start();
        ImGui.SameLine();
        if (ImGui.Button("Stop")) plugin.Engine.Stop();
        ImGui.SameLine();
        if (ImGui.Button("Reload queue")) plugin.Engine.Reload();

        ImGui.Separator();
        var c = plugin.Config;
        Toggle("Daily clan marks", c.RunDailyMarks, v => c.RunDailyMarks = v);
        Toggle("Weekly B-rank bills", c.RunWeeklyBRanks, v => c.RunWeeklyBRanks = v);
        Toggle("HuntHelper A-rank train", c.RunARanks, v => c.RunARanks = v);
        Toggle("Prefer flying", c.PreferFlying, v => c.PreferFlying = v);
        Toggle("Stop on player death", c.StopOnPlayerDeath, v => c.StopOnPlayerDeath = v);

        ImGui.Separator();
        ImGui.TextUnformatted("Expansions");
        Toggle("Shadowbringers", c.EnableShadowbringers, v => c.EnableShadowbringers = v);
        Toggle("Endwalker", c.EnableEndwalker, v => c.EnableEndwalker = v);
        Toggle("Dawntrail", c.EnableDawntrail, v => c.EnableDawntrail = v);

        ImGui.Separator();
        var step = c.PatrolMapStep;
        if (ImGui.SliderFloat("Patrol map step", ref step, 3f, 7f, "%.1f")) { c.PatrolMapStep = step; plugin.SaveConfig(); }
        var radius = c.SearchRadius;
        if (ImGui.SliderFloat("Object scan radius", ref radius, 40f, 150f, "%.0f yalms")) { c.SearchRadius = radius; plugin.SaveConfig(); }
    }

    private void Toggle(string label, bool value, Action<bool> set)
    {
        var v = value;
        if (ImGui.Checkbox(label, ref v))
        {
            set(v);
            plugin.SaveConfig();
        }
    }
}
