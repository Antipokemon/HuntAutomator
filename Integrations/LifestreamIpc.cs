using Dalamud.Plugin.Ipc;
using Lumina.Excel.Sheets;

namespace HuntAutomator.Integrations;

internal sealed class LifestreamIpc
{
    private readonly ICallGateSubscriber<uint, byte, bool> teleport =
        Service.PluginInterface.GetIpcSubscriber<uint, byte, bool>("Lifestream.Teleport");

    public bool TeleportToTerritory(uint territoryId)
    {
        try
        {
            var aetheryte = Service.DataManager.GetExcelSheet<Aetheryte>()
                .Where(x => x.IsAetheryte && x.Territory.ValueNullable?.RowId == territoryId)
                .Select(x => x.RowId)
                .FirstOrDefault();
            return aetheryte != 0 && teleport.InvokeFunc(aetheryte, 0);
        }
        catch (Exception ex)
        {
            Service.Log.Warning(ex, "Lifestream teleport IPC failed");
            return false;
        }
    }
}
