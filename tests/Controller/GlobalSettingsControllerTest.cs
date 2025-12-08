namespace MagpieTest.Controller;

using Magpie.Config;
using Magpie.Controller;
using Magpie.Data;
using MagpieTest.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class GlobalSettingsControllerTest
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DefersIsPluginActiveToActivePluginState(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        var activePlugins = new Mock<IActivePluginState>();
        activePlugins.Setup(_ => _.ActivePlugins).Returns(value ? ["vnavmesh",] : []);
        var gathering = new Mock<IGatheringData>();
        var controller = new GlobalSettingsController(settings.Object, activePlugins.Object, gathering.Object);

        Assert.AreEqual(value, controller.IsPluginActive("vnavmesh"));
    }

    [TestMethod]
    public void DefersRequiredPluginsToActivePluginState()
    {
        var settings = new Mock<IGlobalSettings>();
        var activePlugins = new Mock<IActivePluginState>();
        activePlugins.Setup(_ => _.RequiredPlugins).Returns(["vnavmesh",]);
        var gathering = new Mock<IGatheringData>();
        var controller = new GlobalSettingsController(settings.Object, activePlugins.Object, gathering.Object);

        Assert.Contains("vnavmesh", controller.RequiredPlugins);
        Assert.DoesNotContain("definitely-not-there", controller.RequiredPlugins);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DefersShowOverlayToGlobalSettings(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        settings.Setup(_ => _.ShowOverlay).Returns(value);
        var controller = new GlobalSettingsController(settings.Object, null, null);

        Assert.AreEqual(value, controller.ShowOverlay);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DefersEnableAutoretainerToGlobalSettings(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        settings.Setup(_ => _.EnableAutoretainer).Returns(value);
        var controller = new GlobalSettingsController(settings.Object, null, null);

        Assert.AreEqual(value, controller.EnableAutoretainer);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DefersMultiModeToGlobalSettings(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        settings.Setup(_ => _.AutoRetainerMultiMode).Returns(value);
        var controller = new GlobalSettingsController(settings.Object, null, null);

        Assert.AreEqual(value, controller.MultiMode);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DefersRepairAtNpcToGlobalSettings(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        settings.Setup(_ => _.RepairAtNpc).Returns(value);
        var controller = new GlobalSettingsController(settings.Object, null, null);

        Assert.AreEqual(value, controller.RepairAtNpc);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void WritesShowOverlayToGlobalSettings(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        settings.SetupProperty(_ => _.ShowOverlay);
        var controller = new GlobalSettingsController(settings.Object, null, null);
        controller.ShowOverlay = value;

        Assert.AreEqual(value, settings.Object.ShowOverlay);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void WritesEnableAutoretainerToGlobalSettings(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        settings.SetupProperty(_ => _.EnableAutoretainer);
        var controller = new GlobalSettingsController(settings.Object, null, null);
        controller.EnableAutoretainer = value;

        Assert.AreEqual(value, settings.Object.EnableAutoretainer);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void WritesMultiModeToGlobalSettings(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        settings.SetupProperty(_ => _.AutoRetainerMultiMode);
        var controller = new GlobalSettingsController(settings.Object, null, null);
        controller.MultiMode = value;

        Assert.AreEqual(value, settings.Object.AutoRetainerMultiMode);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void WritesRepairAtNpcToGlobalSettings(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        settings.SetupProperty(_ => _.RepairAtNpc);
        var controller = new GlobalSettingsController(settings.Object, null, null);
        controller.RepairAtNpc = value;

        Assert.AreEqual(value, settings.Object.RepairAtNpc);
    }

    [TestMethod]
    public void DefersGatheringAbilitiesToGatheringData()
    {
        var abilities = new Dictionary<uint, GatheringAbility>()
        {
            { 1, GatheringAbilityFactory.New(1) },
            { 2, GatheringAbilityFactory.New(2) },
        };

        var settings = new Mock<IGlobalSettings>();
        var abilitiesMock = new Mock<IGatheringData>();
        abilitiesMock.Setup(a => a.Abilities).Returns(abilities);

        var controller = new GlobalSettingsController(settings.Object, null, abilitiesMock.Object);

        Assert.AreEqual(2, controller.GatheringAbilities.Count());
    }

    [TestMethod]
    public void DefersMinerAbilitiesToGatheringData()
    {
        var abilities = new Dictionary<uint, GatheringAbility>()
        {
            { 1, GatheringAbilityFactory.New(jobId: 1) },
            { 2, GatheringAbilityFactory.New(jobId: 2) },
        };

        var settings = new Mock<IGlobalSettings>();
        var abilitiesMock = new Mock<IGatheringData>();
        abilitiesMock.Setup(a => a.Abilities).Returns(abilities);
        abilitiesMock.Setup(a => a.MinerJobId).Returns(1);

        var controller = new GlobalSettingsController(settings.Object, null, abilitiesMock.Object);

        Assert.AreEqual(1, controller.MinerAbilities.Count());
    }

    [TestMethod]
    public void DefersBotanistAbilitiesToGatheringData()
    {
        var abilities = new Dictionary<uint, GatheringAbility>()
        {
            { 1, GatheringAbilityFactory.New(jobId: 1) },
            { 2, GatheringAbilityFactory.New(jobId: 2) },
        };

        var settings = new Mock<IGlobalSettings>();
        var abilitiesMock = new Mock<IGatheringData>();
        abilitiesMock.Setup(a => a.Abilities).Returns(abilities);
        abilitiesMock.Setup(a => a.BotanistJobId).Returns(2);

        var controller = new GlobalSettingsController(settings.Object, null, abilitiesMock.Object);

        Assert.AreEqual(1, controller.BotanistAbilities.Count());
    }

    [TestMethod]
    public void DefersMinerJobNameToGatheringData()
    {
        var settings = new Mock<IGlobalSettings>();
        var abilitiesMock = new Mock<IGatheringData>();
        abilitiesMock.Setup(a => a.MinerJobName).Returns("Miner");

        var controller = new GlobalSettingsController(settings.Object, null, abilitiesMock.Object);

        Assert.AreEqual("Miner", controller.MinerJobName);
    }

    [TestMethod]
    public void DefersBotanistJobNameToGatheringData()
    {
        var settings = new Mock<IGlobalSettings>();
        var abilitiesMock = new Mock<IGatheringData>();
        abilitiesMock.Setup(a => a.BotanistJobName).Returns("Botanist");

        var controller = new GlobalSettingsController(settings.Object, null, abilitiesMock.Object);

        Assert.AreEqual("Botanist", controller.BotanistJobName);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DefersIsAbilityEnabledToGlobalSettings(bool value)
    {
        var settings = new Mock<IGlobalSettings>();
        var ability = GatheringAbilityFactory.New();
        settings.Setup(x => x.IsAbilityEnabled(ability)).Returns(value);

        var controller = new GlobalSettingsController(settings.Object, null, null);

        Assert.AreEqual(value, controller.IsAbilityEnabled(ability));
    }

    [TestMethod]
    public void DefersEnablingAbilityToGlobalSettings()
    {
        var settings = new Mock<IGlobalSettings>();

        var ability = GatheringAbilityFactory.New();
        settings.Setup(_ => _.EnableAbility(ability));

        var controller = new GlobalSettingsController(settings.Object, null, null);

        controller.SetAbilityState(ability, true);
        settings.Verify(x => x.EnableAbility(ability));
    }

    [TestMethod]
    public void DefersDisablingAbilityToGlobalSettings()
    {
        var settings = new Mock<IGlobalSettings>();

        var ability = GatheringAbilityFactory.New();
        settings.Setup(x => x.DisableAbility(ability));

        var controller = new GlobalSettingsController(settings.Object, null, null);

        controller.SetAbilityState(ability, false);
        settings.Verify(x => x.DisableAbility(ability));
    }

    [TestMethod]
    public void DefersSavingToGlobalSettings()
    {
        var settings = new Mock<IGlobalSettings>();
        var abilitiesMock = new Mock<IGatheringData>();

        var controller = new GlobalSettingsController(settings.Object, null, abilitiesMock.Object);
        controller.Save();

        settings.Verify(_ => _.Save(), Times.Once());
    }

    [TestMethod]
    public void InvokesToggleWindowEventWhenToggled()
    {
        var settings = new Mock<IGlobalSettings>();

        var controller = new GlobalSettingsController(settings.Object, null, null);

        var toggled = false;
        controller.ToggleWindow += () => toggled = true;

        controller.Toggle();

        Assert.IsTrue(toggled);
    }

    [TestMethod]
    public void DoesNotInvokeToggleWindowEventWhenToggledAndNoListeners()
    {
        var settings = new Mock<IGlobalSettings>();

        var controller = new GlobalSettingsController(settings.Object, null, null);

        controller.Toggle();
    }
}
