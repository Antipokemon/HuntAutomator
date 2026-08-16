namespace HuntAutomator.Integrations;

internal sealed class RotationSolverIpc
{
    // Using RSR's public slash command avoids taking a compile/runtime dependency on its StateCommandType enum.
    public bool IsAvailable() => Service.CommandManager.Commands.ContainsKey("/rotation") || Service.CommandManager.Commands.ContainsKey("/rsr");

    public bool StartAutoBig(uint _)
    {
        try { return Service.CommandManager.ProcessCommand("/rotation Auto Big"); }
        catch (Exception ex) { Service.Log.Warning(ex, "RotationSolverReborn start failed"); return false; }
    }

    public void Stop(uint _)
    {
        try { Service.CommandManager.ProcessCommand("/rotation Off"); }
        catch { }
    }
}
