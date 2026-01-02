namespace MagpieTest.Controller;

using Magpie.Config;
using Magpie.Controller;
using Magpie.Data;
using Magpie.Model;
using Magpie.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Collections.Immutable;

[TestClass]
public class MainControllerTests
{
    [TestMethod]
    public void CallsToggleOnGlobalSettingsWhenCallingToggleConfigWindow()
    {
        var globalSettings = new Mock<IGlobalSettingsController>();
        var listEditor = new Mock<IListEditorController>();
        var repository = new Mock<IListRepository>();
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) { List = list.Object, };

        var controller = new MainController(
            globalSettings.Object,
            listEditor.Object,
            repository.Object,
            currentList
        );

        controller.ToggleConfigWindow();

        globalSettings.Verify(x => x.Toggle(), Times.Once);
    }

    [TestMethod]
    public void SetsCurrentListAndOpensListEditorWhenCallingOpenListEditor()
    {
        var globalSettings = new Mock<IGlobalSettingsController>();
        var listEditor = new Mock<IListEditorController>();
        var repository = new Mock<IListRepository>();
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) { List = GatheringList.Empty, };

        var list = new Mock<IGatheringList>();
        repository.Setup(x => x.GetList("test-list")).Returns(list.Object);

        var controller = new MainController(
            globalSettings.Object,
            listEditor.Object,
            repository.Object,
            currentList
        );

        controller.OpenListEditor("test-list");

        Assert.AreEqual(currentList.List, list.Object);
        listEditor.Verify(x => x.Open(), Times.Once);
    }

    [TestMethod]
    public void CallsRepositoryGetListsWhenCallingGetLists()
    {
        var globalSettings = new Mock<IGlobalSettingsController>();
        var listEditor = new Mock<IListEditorController>();
        var repository = new Mock<IListRepository>();
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) { List = list.Object, };

        const string mockFilter = "test";
        var expectedLists = new List<string>() { "list1", "list2", }.ToImmutableArray();

        repository.Setup(x => x.GetLists(mockFilter)).Returns(expectedLists);

        var controller = new MainController(
            globalSettings.Object,
            listEditor.Object,
            repository.Object,
            currentList
        );

        var result = controller.GetLists(mockFilter);

        Assert.AreEqual(expectedLists, result);
        repository.Verify(x => x.GetLists(mockFilter), Times.Once);
    }

    [TestMethod]
    public void CallsRepositoryDeleteWhenCallingDeleteList()
    {
        var globalSettings = new Mock<IGlobalSettingsController>();
        var listEditor = new Mock<IListEditorController>();
        var repository = new Mock<IListRepository>();
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) { List = list.Object, };

        const string listName = "test-list";

        var controller = new MainController(
            globalSettings.Object,
            listEditor.Object,
            repository.Object,
            currentList
        );

        controller.DeleteList(listName);

        repository.Verify(x => x.DeleteList(listName), Times.Once);
    }

    [TestMethod]
    public void DoesNothingWhenCallingOpenImport()
    {
        var globalSettings = new Mock<IGlobalSettingsController>();
        var listEditor = new Mock<IListEditorController>();
        var repository = new Mock<IListRepository>();
        var recipe = new Mock<IRecipeData>();
        var gathering = new Mock<IGatheringData>();
        var list = new Mock<IGatheringList>();
        var settings = new Mock<IGlobalSettings>();
        var currentList = new CurrentList(gathering.Object, recipe.Object, settings.Object) { List = list.Object, };

        var controller = new MainController(
            globalSettings.Object,
            listEditor.Object,
            repository.Object,
            currentList
        );

        controller.OpenImport();
    }
}
