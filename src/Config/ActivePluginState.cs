namespace Magpie.Config;

using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin;
using ECommons;

public interface IActivePluginState : IDisposable
{
    IEnumerable<string> RequiredPlugins { get; }
    HashSet<string> ActivePlugins { get; }
}

[Autowire(typeof(IActivePluginState))]
public sealed class ActivePluginState : IActivePluginState
{
    public IEnumerable<string> RequiredPlugins { get; } = ["vnavmesh", "Lifestream",];
    public HashSet<string> ActivePlugins { get; internal set; }
    private readonly IDalamudPluginInterface pluginInterface;

    public ActivePluginState(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
        pluginInterface.ActivePluginsChanged += PluginStateChanged;

        var loadedPlugins = from plugin in pluginInterface.InstalledPlugins
                            where RequiredPlugins.Contains(plugin.InternalName) && plugin.IsLoaded
                            select plugin.InternalName;

        ActivePlugins = [.. loadedPlugins,];
    }

    [Shutdown]
    public void Dispose() => pluginInterface.ActivePluginsChanged -= PluginStateChanged;

    private void PluginStateChanged(IActivePluginsChangedEventArgs args)
    {
        var affectedPlugins = RequiredPlugins.Intersect(args.AffectedInternalNames);
        if (!affectedPlugins.Any()) {
            return;
        }

        if (args.Kind == PluginListInvalidationKind.Loaded) {
            ActivePlugins.AddRange(affectedPlugins);
        }
        else if (args.Kind == PluginListInvalidationKind.Unloaded) {
            ActivePlugins.RemoveWhere(_ => affectedPlugins.Contains(_));
        }
    }
}
