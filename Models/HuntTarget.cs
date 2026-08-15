using System.Numerics;

namespace HuntAutomator.Models;

public enum HuntKind { Daily, BRank, ARank }

public sealed record HuntTarget(
    uint MobId,
    string Name,
    uint TerritoryId,
    uint MapId,
    uint ExpansionId,
    HuntKind Kind,
    byte BillNumber,
    byte MobIndex,
    int NeededKills,
    int CurrentKills,
    Vector2? MapPosition = null)
{
    public int RemainingKills => Kind == HuntKind.Daily ? Math.Max(0, NeededKills - CurrentKills) : 1;
}
