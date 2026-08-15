using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using HuntAutomator.Core;
using HuntAutomator.UI;

namespace HuntAutomator;

public sealed class Plugin : IDalamudPlugin
{
    private readonly WindowSystem windows = new("HuntAutomator");
    private readonly MainWindow main;
    public Configuration Config { get; }
    internal HuntEngine Engine { get; }

    public Plugin(IDalamudPluginInterface pi)
    {
        pi.Create<Service>();
        Config = pi.GetPluginConfig() as Configuration ?? new Configuration();
        Engine = new HuntEngine(Config);
        main = new MainWindow(this);
        windows.AddWindow(main);

        Service.CommandManager.AddHandler("/hauto", new CommandInfo(OnCommand)
        {
            HelpMessage = "Open Hunt Automator; /hauto start|stop|reload"
        });
        Service.Framework.Update += OnUpdate;
        pi.UiBuilder.Draw += windows.Draw;
        pi.UiBuilder.OpenMainUi += main.Toggle;
        pi.UiBuilder.OpenConfigUi += main.Toggle;
    }

    private void OnCommand(string _, string args)
    {
        switch (args.Trim().ToLowerInvariant())
        {
            case "start": Engine.Start(); break;
            case "stop": Engine.Stop(); break;
            case "reload": Engine.Reload(); break;
            default: main.Toggle(); break;
        }
    }

    private void OnUpdate(IFramework _) => Engine.Tick();
    internal void SaveConfig() => Service.PluginInterface.SavePluginConfig(Config);

    public void Dispose()
    {
        Engine.Stop();
        Service.Framework.Update -= OnUpdate;
        Service.CommandManager.RemoveHandler("/hauto");
        Service.PluginInterface.UiBuilder.Draw -= windows.Draw;
        Service.PluginInterface.UiBuilder.OpenMainUi -= main.Toggle;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= main.Toggle;
        windows.RemoveAllWindows();
    }
}
