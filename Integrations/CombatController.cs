using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace HuntAutomator.Integrations;

internal static class CombatController
{
    private const uint AutoAttackGeneralActionId = 1;

    public static unsafe bool TryAutoAttack(IGameObject target)
    {
        var actions = ActionManager.Instance();
        return actions != null &&
               actions->UseAction(ActionType.GeneralAction, AutoAttackGeneralActionId, target.GameObjectId);
    }
}
