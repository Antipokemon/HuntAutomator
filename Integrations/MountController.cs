using FFXIVClientStructs.FFXIV.Client.Game;

namespace HuntAutomator.Integrations;

internal static class MountController
{
    private const uint MountRouletteGeneralActionId = 9;

    public static unsafe bool TryMount()
    {
        var actions = ActionManager.Instance();
        return actions != null &&
               actions->UseAction(ActionType.GeneralAction, MountRouletteGeneralActionId);
    }
}
