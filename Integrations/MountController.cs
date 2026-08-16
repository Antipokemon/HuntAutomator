using FFXIVClientStructs.FFXIV.Client.Game;

namespace HuntAutomator.Integrations;

internal static class MountController
{
    private const uint MountRouletteGeneralActionId = 9;
    private const uint DismountGeneralActionId = 23;

    public static unsafe bool TryMount()
    {
        var actions = ActionManager.Instance();
        return actions != null &&
               actions->UseAction(ActionType.GeneralAction, MountRouletteGeneralActionId);
    }

    public static unsafe bool TryDismount()
    {
        var actions = ActionManager.Instance();
        return actions != null &&
               actions->UseAction(ActionType.GeneralAction, DismountGeneralActionId);
    }
}
