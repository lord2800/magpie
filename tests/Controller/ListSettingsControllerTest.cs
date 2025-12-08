namespace MagpieTest.Controller;

using Magpie.Config;
using Magpie.Controller;
using Magpie.Data;
using Magpie.Model;
using Magpie.Services;
using MagpieTest.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class ListSettingsControllerTest
{
    [TestMethod]
    public void DefersToGatheringForGatheringAbilities()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        gathering.Setup(g => g.Abilities)
            .Returns(new Dictionary<uint, GatheringAbility>() {
                { 1u, GatheringAbilityFactory.New(id: 1, jobId: 1) },
                { 2u, GatheringAbilityFactory.New(id: 2, jobId: 2) },
            });

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        Assert.AreEqual(2, controller.GatheringAbilities.Count());
    }

    [TestMethod]
    public void DefersToGatheringForMinerSpecificAbilities()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        gathering.Setup(g => g.Abilities)
            .Returns(new Dictionary<uint, GatheringAbility>() {
                { 1u, GatheringAbilityFactory.New(id: 1, jobId: 1) },
                { 2u, GatheringAbilityFactory.New(id: 2, jobId: 2) },
            });
        gathering.SetupGet(g => g.MinerJobId).Returns(1);

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        // Assuming MinerJobId is 1
        Assert.IsTrue(controller.MinerAbilities.Any(a => a.JobId == 1));
    }

    [TestMethod]
    public void DefersToGatheringForBotanistSpecificAbilities()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        gathering.Setup(g => g.Abilities)
            .Returns(new Dictionary<uint, GatheringAbility>() {
                { 1u, GatheringAbilityFactory.New(id: 1, jobId: 1) },
                { 2u, GatheringAbilityFactory.New(id: 2, jobId: 2) },
            });
        gathering.SetupGet(g => g.BotanistJobId).Returns(1);

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        // Assuming MinerJobId is 1
        Assert.IsTrue(controller.BotanistAbilities.Any(a => a.JobId == 1));
    }

    [TestMethod]
    public void DefersToGatheringForMinerJobName()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        gathering.SetupGet(g => g.MinerJobName).Returns("Miner");

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        // Assuming MinerJobId is 1
        Assert.AreEqual("Miner", controller.MinerJobName);
    }

    [TestMethod]
    public void DefersToGatheringForBotanistJobName()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        gathering.SetupGet(g => g.BotanistJobName).Returns("Miner");

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        // Assuming MinerJobId is 1
        Assert.AreEqual("Miner", controller.BotanistJobName);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DefersToListWhenCallingIsAbilityEnabled(bool value)
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        var ability = GatheringAbilityFactory.New();
        list.Setup(_ => _.IsAbilityEnabled(ability)).Returns(value);

        Assert.AreEqual(value, controller.IsAbilityEnabled(ability));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void TogglesAbilityOntoList(bool state)
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        var ability = GatheringAbilityFactory.New(id: 1);
        controller.SetAbilityState(ability, state);

        list.Verify(l => l.ToggleAbility(ability, state), Times.Once());
    }

    [TestMethod]
    public void RaisesOpenWindowEvent()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        var eventFired = false;
        controller.OpenWindow += () => eventFired = true;

        controller.Open();

        Assert.IsTrue(eventFired);
    }

    [TestMethod]
    public void DoesNotRaiseOpenWindowEventWhenNoListeners()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        controller.Open();
    }

    [TestMethod]
    public void RaisesNameUpdatedEventWhenListNameUpdatedEventTriggered()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        var triggered = false;
        controller.NameUpdated += _ => triggered = true;

        currentList.Name = "updated";

        Assert.IsTrue(triggered);
    }

    [TestMethod]
    public void DoesNotRaiseNameUpdatedEventWhenListNameUpdatedEventTriggeredWithNoListeners()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListSettingsController(
            currentList,
            gathering.Object
        );

        currentList.Name = "updated";
    }
}
