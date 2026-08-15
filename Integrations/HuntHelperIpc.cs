using System.Collections;
using System.Numerics;
using Dalamud.Plugin.Ipc;

namespace HuntAutomator.Integrations;

internal sealed class HuntHelperIpc
{
    private readonly ICallGateSubscriber<object> getTrain = Service.PluginInterface.GetIpcSubscriber<object>("HH.GetTrainList");

    public List<TrainMark> TryGetTrain()
    {
        var result = new List<TrainMark>();
        try
        {
            if (getTrain.InvokeFunc() is not IEnumerable list) return result;
            foreach (var item in list)
            {
                if (item is null) continue;
                var t = item.GetType();
                T? Read<T>(string n) => (T?)t.GetProperty(n)?.GetValue(item);
                result.Add(new TrainMark(
                    Read<string>("Name") ?? "Unknown",
                    Read<uint>("MobID"), Read<uint>("TerritoryID"), Read<uint>("MapID"),
                    Read<uint>("Instance"), Read<Vector2>("Position"), Read<bool>("Dead")));
            }
        }
        catch { }
        return result;
    }
}

internal sealed record TrainMark(string Name, uint MobId, uint TerritoryId, uint MapId, uint Instance, Vector2 Position, bool Dead);
