namespace MagpieTest.Config;

using Magpie.Config;
using MagpieTest.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

[TestClass]
public sealed class GlobalSettingsTest
{
    [TestMethod]
    public void InvokesOverlayStateChangedWhenChangingShowOverlayValue()
    {
        var settings = new GlobalSettings();
        var mock = new Mock<Action<bool>>();
        settings.OverlayStateChanged += mock.Object;
        settings.ShowOverlay = true;
        mock.Verify(f => f(true));
        settings.ShowOverlay = false;
        mock.Verify(f => f(false));
    }

    [TestMethod]
    public void ReturnsCurrentOverlayState()
    {
        var settings = new GlobalSettings() {
            ShowOverlay = true,
        };
        Assert.IsTrue(settings.ShowOverlay);
        settings.ShowOverlay = false;
        Assert.IsFalse(settings.ShowOverlay);
    }

    [TestMethod]
    public void ReturnsCurrentAutoretainerState()
    {
        var settings = new GlobalSettings();
        settings.EnableAutoretainer = true;
        Assert.IsTrue(settings.EnableAutoretainer);
        settings.EnableAutoretainer = false;
        Assert.IsFalse(settings.EnableAutoretainer);
    }

    [TestMethod]
    public void ReturnsCurrentMultiModeState()
    {
        var settings = new GlobalSettings();
        settings.AutoRetainerMultiMode = true;
        Assert.IsTrue(settings.AutoRetainerMultiMode);
        settings.AutoRetainerMultiMode = false;
        Assert.IsFalse(settings.AutoRetainerMultiMode);
    }

    [TestMethod]
    public void ReturnsCurrentRepairAtNpcState()
    {
        var settings = new GlobalSettings();
        settings.RepairAtNpc = true;
        Assert.IsTrue(settings.RepairAtNpc);
        settings.RepairAtNpc = false;
        Assert.IsFalse(settings.RepairAtNpc);
    }

    [TestMethod]
    public void DoesNotInvokeOverlayStateChangedWhenChangingShowOverlayWithNoListeners()
    {
        var settings = new GlobalSettings();
        settings.ShowOverlay = true;
        settings.ShowOverlay = false;
    }

    [TestMethod]
    public void ManagesAbilitiesCorrectly()
    {
        var settings = new GlobalSettings();

        var ability = GatheringAbilityFactory.New();

        // default state: abilities are all disabled
        Assert.IsFalse(settings.IsAbilityEnabled(ability));

        settings.EnableAbility(ability);
        Assert.IsTrue(settings.IsAbilityEnabled(ability));

        settings.DisableAbility(ability);
        Assert.IsFalse(settings.IsAbilityEnabled(ability));
    }
}
