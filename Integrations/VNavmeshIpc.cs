using System.Numerics;
using Dalamud.Plugin.Ipc;

namespace HuntAutomator.Integrations;

internal sealed class VNavmeshIpc
{
    private readonly ICallGateSubscriber<bool> ready = Service.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.Nav.IsReady");
    private readonly ICallGateSubscriber<Vector3, bool, float, bool> moveClose = Service.PluginInterface.GetIpcSubscriber<Vector3, bool, float, bool>("vnavmesh.SimpleMove.PathfindAndMoveCloseTo");
    private readonly ICallGateSubscriber<bool> moving = Service.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.Path.IsRunning");
    private readonly ICallGateSubscriber<bool> pathfinding = Service.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.SimpleMove.PathfindInProgress");
    private readonly ICallGateSubscriber<object> stop = Service.PluginInterface.GetIpcSubscriber<object>("vnavmesh.Path.Stop");
    private readonly ICallGateSubscriber<Vector3, float, float, Vector3?> nearest = Service.PluginInterface.GetIpcSubscriber<Vector3, float, float, Vector3?>("vnavmesh.Query.Mesh.NearestPoint");

    public bool IsReady() { try { return ready.InvokeFunc(); } catch { return false; } }
    public bool IsBusy() { try { return moving.InvokeFunc() || pathfinding.InvokeFunc(); } catch { return false; } }
    public bool MoveTo(Vector3 p, bool fly, float range) { try { return moveClose.InvokeFunc(p, fly, range); } catch { return false; } }
    public Vector3? SnapToMesh(Vector3 p) { try { return nearest.InvokeFunc(p, 80f, 500f); } catch { return null; } }
    public void Stop() { try { stop.InvokeAction(); } catch { } }
}
