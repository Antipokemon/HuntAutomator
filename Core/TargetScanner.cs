using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;

namespace HuntAutomator.Core;

internal static class TargetScanner
{
    public static IGameObject? Find(uint nameId, float radius)
    {
        var player = Service.ObjectTable.LocalPlayer;
        if (player is null) return null;
        return Service.ObjectTable
            .Where(o => o.ObjectKind == ObjectKind.BattleNpc && o.IsTargetable && o.NameId == nameId)
            .OrderBy(o => System.Numerics.Vector3.DistanceSquared(o.Position, player.Position))
            .FirstOrDefault(o => System.Numerics.Vector3.Distance(o.Position, player.Position) <= radius);
    }
}
