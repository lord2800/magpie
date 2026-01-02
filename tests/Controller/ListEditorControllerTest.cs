namespace MagpieTest.Controller;

using Magpie.Config;
using Magpie.Controller;
using Magpie.Data;
using Magpie.Model;
using Magpie.Services;
using MagpieTest.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Frozen;
using System.Collections.Generic;

[TestClass]
public class ListEditorControllerTest
{
    [TestMethod]
    public void ReturnsCurrentListWhenGettingListProperty()
    {
        var recipe = new Mock<IRecipeData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var gathering = new Mock<IGatheringData>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            new Mock<IListRepository>().Object
        );

        Assert.AreEqual(currentList, controller.List);
    }

    // this test is going to be a pain to write, do it later
    // [TestMethod]
    // public void DefersToCurrentListForGatherables()
    // {
    //     var recipe = new Mock<IRecipeData>();
    //     var gathering = new Mock<IGatheringData>();
    //     var list = new Mock<IGatheringList>();
    //     var settings = new Mock<IGlobalSettings>();
    //     var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
    //         List = list.Object,
    //     };
    //     var repository = new Mock<IListRepository>();

    //     var controller = new ListEditorController(
    //         new Mock<IListSettingsController>().Object,
    //         currentList,
    //         recipe.Object,
    //         gathering.Object,
    //         repository.Object
    //     );
    // }

    [TestMethod]
    public void DefersToRecipeDataWhenGettingRecipesProperty()
    {
        var recipeList = new Dictionary<uint, Recipe>() {
            {1, RecipeFactory.New(id: 1)},
        }.ToFrozenDictionary();
        var recipe = new Mock<IRecipeData>();
        recipe.SetupGet(r => r.Recipes).Returns(recipeList);
        var gathering = new Mock<IGatheringData>();

        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = new Mock<IGatheringList>().Object,
        };

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            new Mock<IListRepository>().Object
        );

        Assert.AreEqual(recipeList, controller.Recipes);
    }

    [TestMethod]
    public void DefersToGatheringDataWhenGettingItemsProperty()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = new Mock<IGatheringList>().Object,
        };
        var items = new Dictionary<uint, GatheringItem>() {
            { 1, GatheringItemFactory.New() },
        };
        gathering.SetupGet(g => g.GatheringItems).Returns(items);

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            new Mock<IListRepository>().Object
        );

        Assert.AreEqual(items, controller.GatheringItems);
    }

    [TestMethod]
    public void CallsOpenOnListSettingsControllerWhenCallingOpen()
    {
        var listSettings = new Mock<IListSettingsController>();
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListEditorController(
            listSettings.Object,
            currentList,
            recipe.Object,
            gathering.Object,
            new Mock<IListRepository>().Object
        );

        controller.OpenSettings();

        listSettings.Verify(m => m.Open(), Times.Once());
    }

    [TestMethod]
    public void CreatesProductItemTypeWhenAddingRecipe()
    {
        var list = new Mock<IGatheringList>();
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            new Mock<IListRepository>().Object
        );

        controller.AddToList(RecipeFactory.New(), 5);

        list.Verify(
            m => m.Add(new GatheringListItem() {
                Type = GatheringItemType.Product,
                ItemId = 1,
                Quantity = 5,
                Collected = 0,
            }), Times.Once());
    }

    [TestMethod]
    public void CreatesGatheredItemTypeWhenAddingGatheringItem()
    {
        var list = new Mock<IGatheringList>();
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            new Mock<IListRepository>().Object
        );

        controller.AddToList(GatheringItemFactory.New(id: 2), 3);

        list.Verify(
            m => m.Add(new GatheringListItem() {
                Type = GatheringItemType.Gathered,
                ItemId = 2,
                Quantity = 3,
                Collected = 0,
            }), Times.Once());
    }

    [TestMethod]
    public void DoesNothingWhenSavingIfListIsEmpty()
    {
        var recipe = new Mock<IRecipeData>();
        var settings = new Mock<IGlobalSettings>();
        var gathering = new Mock<IGatheringData>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = GatheringList.Empty,
        };
        var repository = new Mock<IListRepository>();

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            new Mock<IRecipeData>().Object,
            gathering.Object,
            repository.Object
        );

        controller.SaveList();

        repository.Verify(m => m.SaveList(It.IsAny<GatheringList>()), Times.Never());
    }

    [TestMethod]
    public void SavesToListRepositoryIfListIsNotEmpty()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        var repository = new Mock<IListRepository>();
        list.SetupGet(_ => _.Name).Returns("not empty");
        list.SetupGet(_ => _.Items).Returns([GatheringListItemFactory.New(),]);

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            repository.Object
        );

        controller.SaveList();

        repository.Verify(m => m.SaveList(list.Object), Times.Once());
    }

    [TestMethod]
    public void InvokesToggleWindowEventWhenToggled()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        var repository = new Mock<IListRepository>();

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            repository.Object
        );

        var toggled = false;
        controller.OpenWindow += () => toggled = true;

        controller.Open();

        Assert.IsTrue(toggled);
    }

    [TestMethod]
    public void DoesNotInvokeToggleWindowEventWhenToggledWithNoListeners()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        var repository = new Mock<IListRepository>();

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            repository.Object
        );

        controller.Open();
    }

    [TestMethod]
    public void InvokesNameUpdatedEventWhenListNameTriggered()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        var repository = new Mock<IListRepository>();

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            repository.Object
        );
        controller.Initialize();

        var called = false;
        controller.NameUpdated += (_) => called = true;

        currentList.Name = "new name";

        Assert.IsTrue(called);
    }

    [TestMethod]
    public void DoesNotInvokeNameUpdatedEventWhenListNameTriggeredWithNoListeners()
    {
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) {
            List = list.Object,
        };
        var repository = new Mock<IListRepository>();

        var controller = new ListEditorController(
            new Mock<IListSettingsController>().Object,
            currentList,
            recipe.Object,
            gathering.Object,
            repository.Object
        );

        currentList.Name = "new name";
    }
}
