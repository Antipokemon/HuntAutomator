using Dalamud.Configuration;

namespace HuntAutomator;

public sealed class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool EnableShadowbringers { get; set; } = true;
    public bool EnableEndwalker { get; set; } = true;
    public bool EnableDawntrail { get; set; } = true;
    public bool RunDailyMarks { get; set; } = true;
    public bool RunWeeklyBRanks { get; set; } = true;
    public bool RunARanks { get; set; } = false;
    public bool PreferFlying { get; set; } = true;
    public float SearchRadius { get; set; } = 90f;
    public float ApproachRange { get; set; } = 4.5f;
    public float PatrolMapStep { get; set; } = 4.5f;
    public int NavigationTimeoutSeconds { get; set; } = 90;
    public int CombatTimeoutSeconds { get; set; } = 360;
    public int MaxRetriesPerTarget { get; set; } = 2;
    public bool UseHuntHelperTrain { get; set; } = true;
    public bool StopOnPlayerDeath { get; set; } = true;
}
