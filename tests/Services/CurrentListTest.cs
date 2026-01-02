namespace MagpieTest.Services;

using Magpie.Config;
using Magpie.Data;
using Magpie.Model;
using Magpie.Services;
using MagpieTest.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public sealed class CurrentListTest
{
    [TestMethod]
    public void DefersBasicPropertiesToAssignedList()
    {
        const string expectedName = "test";
        var expectedAbilities = new List<GatheringListAbility>() {
            GatheringListAbilityFactory.New(GatheringAbilityFactory.New()),
        };
        var expectedItems = new List<GatheringListItem>() {
            GatheringListItemFactory.New(),
        };

        var gatheringList = new Mock<IGatheringList>();

        gatheringList.Setup(_ => _.Name).Returns(expectedName);
        gatheringList.Setup(_ => _.Abilities).Returns(expectedAbilities);
        gatheringList.Setup(_ => _.Items).Returns(expectedItems);

        var recipe = RecipeFactory.New();
        var recipes = new Dictionary<uint, Recipe>() {
            { 1, recipe },
        }.ToFrozenDictionary();
        var recipeData = new Mock<IRecipeData>();
        recipeData.Setup(_ => _.Recipes).Returns(recipes);
        recipeData.Setup(_ => _.GetAllIngredients(recipe)).Returns(new List<Ingredient>());

        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, recipeData.Object, settings.Object) {
            List = gatheringList.Object,
        };

        Assert.AreEqual(expectedName, currentList.Name);
        Assert.AreEqual(expectedAbilities, currentList.Abilities);
        Assert.AreEqual(expectedItems, currentList.Items);
    }

    [TestMethod]
    public void DelegatesAddItemToAssignedList()
    {
        var list = new Mock<IGatheringList>();
        var item = GatheringListItemFactory.New();

        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, new Mock<IRecipeData>().Object, settings.Object) { List = list.Object, };

        currentList.Add(item);

        list.Verify(x => x.Add(item), Times.Once);
    }

    [TestMethod]
    public void DelegatesIsAbilityEnabledToAssignedList()
    {
        var ability = GatheringAbilityFactory.New();
        var list = new Mock<IGatheringList>();

        list.Setup(x => x.IsAbilityEnabled(ability)).Returns(true);

        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, new Mock<IRecipeData>().Object, settings.Object) { List = list.Object, };

        Assert.IsTrue(currentList.IsAbilityEnabled(ability));
    }

    [TestMethod]
    public void DelegatesRemoveItemToAssignedList()
    {
        var item = GatheringListItemFactory.New();
        var list = new Mock<IGatheringList>();

        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, new Mock<IRecipeData>().Object, settings.Object) { List = list.Object, };

        currentList.Remove(item);

        list.Verify(x => x.Remove(item), Times.Once);
    }

    [TestMethod]
    public void DelegatesCollectItemToAssignedList()
    {
        var item = GatheringListItemFactory.New();
        const int quantity = 5;

        var list = new Mock<IGatheringList>();

        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, new Mock<IRecipeData>().Object, settings.Object) { List = list.Object, };

        currentList.Collect(item, quantity);

        list.Verify(x => x.Collect(item, quantity), Times.Once);
    }

    [TestMethod]
    public void DelegatesResetToAssignedList()
    {
        var list = new Mock<IGatheringList>();

        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, new Mock<IRecipeData>().Object, settings.Object) { List = list.Object, };

        currentList.Reset();

        list.Verify(x => x.Reset(), Times.Once);
    }

    [TestMethod]
    public void DelegatesToggleAbilityToAssignedList()
    {
        var ability = GatheringAbilityFactory.New();
        const bool enable = true;

        var list = new Mock<IGatheringList>();

        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, new Mock<IRecipeData>().Object, settings.Object) { List = list.Object, };

        currentList.ToggleAbility(ability, enable);

        list.Verify(x => x.ToggleAbility(ability, enable), Times.Once);
    }

    [TestMethod]
    public void TriggersNameUpdatedEventOnNameBeingUpdated()
    {
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, new Mock<IRecipeData>().Object, settings.Object) { List = list.Object, };

        var handled = false;
        currentList.NameUpdated += () => handled = true;

        currentList.Name = "test";

        Assert.IsTrue(handled);
    }

    [TestMethod]
    public void DoesNotTriggerNameUpdatedEventOnNameBeingUpdatedWithNoListeners()
    {
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, new Mock<IRecipeData>().Object, settings.Object) { List = list.Object, };

        currentList.Name = "test";
    }

    [TestMethod]
    public void CalculatesGatherablesBasedOnRecipeData()
    {
        var recipe = RecipeFactory.New();
        var item = GatheringListItemFactory.New();
        var materials = new List<Ingredient>() { IngredientFactory.New(), };

        var recipes = new Dictionary<uint, Recipe>() {
            { item.ItemId, recipe },
        }.ToFrozenDictionary();

        var recipeData = new Mock<IRecipeData>();
        recipeData.Setup(x => x.Recipes).Returns(recipes);
        recipeData.Setup(x => x.GetAllIngredients(recipe)).Returns(materials);

        var list = new Mock<IGatheringList>();
        list.Setup(x => x.Items).Returns(new List<GatheringListItem>() { item, });
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(new Mock<IGatheringData>().Object, recipeData.Object, settings.Object) {
            List = list.Object,
        };


        Assert.AreEqual(1, currentList.Gatherables.Count());
    }

    [TestMethod]
    public void DefersIsAbilityEnabledToGlobalSettingWhenAbilityNotInList()
    {
        var settings = new Mock<IGlobalSettings>();
        settings.Setup(s => s.IsAbilityEnabled(It.IsAny<GatheringAbility>())).Returns(true);

        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();

        var currentList = new CurrentList(
            gathering: gathering.Object,
            recipe: recipe.Object,
            settings: settings.Object
        );

        Assert.IsTrue(currentList.IsAbilityEnabled(GatheringAbilityFactory.New()));
    }

    [TestMethod]
    public void TestIsAbilityEnabled_WhenAbilityInList_ReturnsListSetting()
    {
        var ability = GatheringAbilityFactory.New();
        var settings = new Mock<IGlobalSettings>();

        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();

        var list = new Mock<IGatheringList>();
        list.Setup(_ => _.IsAbilityEnabled(ability)).Returns(true);
        settings.Setup(s => s.IsAbilityEnabled(ability)).Returns(false);

        var currentList = new CurrentList(
            gathering: gathering.Object,
            recipe: recipe.Object,
            settings: settings.Object
        ) {
            List = list.Object,
        };
        Assert.IsTrue(currentList.IsAbilityEnabled(ability));
    }
}
