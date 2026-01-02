namespace MagpieTest.Services;

using Magpie.Services;
using TraitSheet = Lumina.Excel.Sheets.Trait;
using ActionSheet = Lumina.Excel.Sheets.Action;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Dalamud.Plugin.Services;
using MagpieTest.Factories;

#pragma warning disable Dalamud001
[TestClass]
public class ActionStateTest
{
    [TestMethod]
    public void DelegatesToUnderlyingMethodWhenTraitIsUnlocked()
    {
        var traitSheet = new TraitSheet(null, 0, 1);
        var unlockState = new Mock<IUnlockState>();
        unlockState.Setup(u => u.IsTraitUnlocked(traitSheet)).Returns(true);

        var actionState = new ActionState(unlockState.Object, []);

        Assert.IsTrue(actionState.IsAvailable(traitSheet));
    }

    [TestMethod]
    public void DelegatesToUnderlyingMethodWhenTraitIsLocked()
    {
        var traitSheet = new TraitSheet(null, 0, 1);
        var unlockState = new Mock<IUnlockState>();
        unlockState.Setup(u => u.IsTraitUnlocked(traitSheet)).Returns(false);

        var actionState = new ActionState(unlockState.Object, []);

        Assert.IsFalse(actionState.IsAvailable(traitSheet));
    }

    [TestMethod]
    public void DelegatesToUnderlyingMethodWhenActionIsUnlocked()
    {
        var actionSheet = new ActionSheet(null, 0, 1);
        var unlockState = new Mock<IUnlockState>();
        unlockState.Setup(u => u.IsActionUnlocked(actionSheet)).Returns(true);

        var actionState = new ActionState(unlockState.Object, []);

        Assert.IsTrue(actionState.IsAvailable(actionSheet));
    }

    [TestMethod]
    public void DelegatesToUnderlyingMethodWhenActionIsLocked()
    {
        var actionSheet = new ActionSheet(null, 0, 1);
        var unlockState = new Mock<IUnlockState>();
        unlockState.Setup(u => u.IsActionUnlocked(actionSheet)).Returns(false);

        var actionState = new ActionState(unlockState.Object, []);

        Assert.IsFalse(actionState.IsAvailable(actionSheet));
    }

    [TestMethod]
    public void DelegatesToUnderlyingMethodWhenGatheringAbilityIsUnlocked()
    {
        var ability = GatheringAbilityFactory.New();
        var actionSheet = new ActionSheet(null, 0, 1);
        var unlockState = new Mock<IUnlockState>();
        unlockState.Setup(u => u.IsActionUnlocked(actionSheet)).Returns(true);

        var actionState = new ActionState(unlockState.Object, [actionSheet,]);

        Assert.IsTrue(actionState.IsAvailable(ability));
    }

    [TestMethod]
    public void DelegatesToUnderlyingMethodWhenGatheringAbilityIsLocked()
    {
        var ability = GatheringAbilityFactory.New();
        var actionSheet = new ActionSheet(null, 0, 1);
        var unlockState = new Mock<IUnlockState>();
        unlockState.Setup(u => u.IsActionUnlocked(actionSheet)).Returns(false);

        var dataManager = new Mock<IDataManager>();
        var actionState = new ActionState(unlockState.Object, [actionSheet,]);

        Assert.IsFalse(actionState.IsAvailable(ability));
    }
}

#pragma warning restore Dalamud001
