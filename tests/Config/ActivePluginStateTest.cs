namespace MagpieTest.Config;

using System.Collections.Generic;
using Dalamud.Plugin;
using Magpie.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class ActivePluginStateTest
{
    public static IEnumerable<object[]> RequiredPlugins { get; } = [
        ["vnavmesh",],
        ["Lifestream",],
    ];

    [TestMethod]
    [DynamicData(nameof(RequiredPlugins))]
    public void RecognizesPluginMissingWhenNotLoadedUpfront(string name)
    {
        var pluginInterface = new Mock<IDalamudPluginInterface>();
        pluginInterface.Setup(_ => _.InstalledPlugins).Returns([]);
        var activePlugins = new ActivePluginState(pluginInterface.Object);
        Assert.DoesNotContain(name, activePlugins.ActivePlugins);
        activePlugins.Dispose();
    }

    [TestMethod]
    [DynamicData(nameof(RequiredPlugins))]
    public void RecognizesPluginNotPresentWhenInstalledButNotLoadedUpfront(string name)
    {
        var pluginInterface = new Mock<IDalamudPluginInterface>();
        var plugin = new Mock<IExposedPlugin>();
        plugin.Setup(_ => _.InternalName).Returns(name);
        plugin.Setup(_ => _.IsLoaded).Returns(false);
        pluginInterface.Setup(_ => _.InstalledPlugins).Returns([plugin.Object,]);

        var activePlugins = new ActivePluginState(pluginInterface.Object);
        Assert.DoesNotContain(name, activePlugins.ActivePlugins);
        activePlugins.Dispose();
    }

    [TestMethod]
    [DynamicData(nameof(RequiredPlugins))]
    public void RecognizesPluginPresentWhenLoadedUpfront(string name)
    {
        var pluginInterface = new Mock<IDalamudPluginInterface>();
        var plugin = new Mock<IExposedPlugin>();
        plugin.Setup(_ => _.InternalName).Returns(name);
        plugin.Setup(_ => _.IsLoaded).Returns(true);
        pluginInterface.Setup(_ => _.InstalledPlugins).Returns([plugin.Object,]);

        var activePlugins = new ActivePluginState(pluginInterface.Object);
        Assert.Contains(name, activePlugins.ActivePlugins);
        activePlugins.Dispose();
    }

    [TestMethod]
    [DynamicData(nameof(RequiredPlugins))]
    public void RecognizesPluginPresentWhenLoadedAfterInit(string name)
    {
        var pluginInterface = new Mock<IDalamudPluginInterface>();
        pluginInterface.Setup(_ => _.InstalledPlugins).Returns([]);

        var activePlugins = new ActivePluginState(pluginInterface.Object);
        Assert.DoesNotContain(name, activePlugins.ActivePlugins);

        var evt = new Mock<IActivePluginsChangedEventArgs>();
        evt.Setup(_ => _.AffectedInternalNames).Returns([name,]);
        evt.Setup(_ => _.Kind).Returns(PluginListInvalidationKind.Loaded);

        pluginInterface.Raise(_ => _.ActivePluginsChanged += null, evt.Object);
        Assert.Contains(name, activePlugins.ActivePlugins);
        activePlugins.Dispose();
    }

    [TestMethod]
    [DynamicData(nameof(RequiredPlugins))]
    public void RecognizesPluginNotPresentWhenUnloadedAfterInit(string name)
    {
        var pluginInterface = new Mock<IDalamudPluginInterface>();
        var plugin = new Mock<IExposedPlugin>();
        plugin.Setup(_ => _.InternalName).Returns(name);
        plugin.Setup(_ => _.IsLoaded).Returns(true);
        pluginInterface.Setup(_ => _.InstalledPlugins).Returns([plugin.Object,]);

        var activePlugins = new ActivePluginState(pluginInterface.Object);
        Assert.Contains(name, activePlugins.ActivePlugins);

        var evt = new Mock<IActivePluginsChangedEventArgs>();
        evt.Setup(_ => _.AffectedInternalNames).Returns([name,]);
        evt.Setup(_ => _.Kind).Returns(PluginListInvalidationKind.Unloaded);

        pluginInterface.Raise(_ => _.ActivePluginsChanged += null, evt.Object);
        Assert.DoesNotContain(name, activePlugins.ActivePlugins);
        activePlugins.Dispose();
    }

    [TestMethod]
    [DataRow(PluginListInvalidationKind.Update)]
    [DataRow(PluginListInvalidationKind.AutoUpdate)]
    public void UsesCurrentPluginActiveStateWhenUpdatingOrAutoUpdating(PluginListInvalidationKind kind)
    {
        var pluginInterface = new Mock<IDalamudPluginInterface>();
        var plugin = new Mock<IExposedPlugin>();
        plugin.Setup(_ => _.InternalName).Returns("vnavmesh");
        plugin.Setup(_ => _.IsLoaded).Returns(true);
        pluginInterface.Setup(_ => _.InstalledPlugins).Returns([plugin.Object,]);

        var activePlugins = new ActivePluginState(pluginInterface.Object);
        Assert.Contains("vnavmesh", activePlugins.ActivePlugins);

        var evt = new Mock<IActivePluginsChangedEventArgs>();
        evt.Setup(_ => _.AffectedInternalNames).Returns(["vnavmesh",]);
        evt.Setup(_ => _.Kind).Returns(kind);

        pluginInterface.Raise(_ => _.ActivePluginsChanged += null, evt.Object);
        Assert.Contains("vnavmesh", activePlugins.ActivePlugins);
        activePlugins.Dispose();
    }

    [TestMethod]
    [DataRow(PluginListInvalidationKind.Loaded)]
    [DataRow(PluginListInvalidationKind.Unloaded)]
    [DataRow(PluginListInvalidationKind.Update)]
    [DataRow(PluginListInvalidationKind.AutoUpdate)]
    public void IgnoresPluginStateChangedWhenNoneOfRequiredPluginsAffected(PluginListInvalidationKind kind)
    {
        var pluginInterface = new Mock<IDalamudPluginInterface>();
        var vnav = new Mock<IExposedPlugin>();
        vnav.Setup(_ => _.InternalName).Returns("vnavmesh");
        vnav.Setup(_ => _.IsLoaded).Returns(true);
        pluginInterface.Setup(_ => _.InstalledPlugins).Returns([vnav.Object,]);

        var activePlugins = new ActivePluginState(pluginInterface.Object);
        Assert.Contains("vnavmesh", activePlugins.ActivePlugins);

        var evt = new Mock<IActivePluginsChangedEventArgs>();
        evt.Setup(_ => _.AffectedInternalNames).Returns(["boogeyman",]);
        evt.Setup(_ => _.Kind).Returns(kind);

        pluginInterface.Raise(_ => _.ActivePluginsChanged += null, evt.Object);
        Assert.Contains("vnavmesh", activePlugins.ActivePlugins);
        activePlugins.Dispose();
    }
}
