namespace MagpieTest.Services;

using Magpie.Data;
using Magpie.Model;
using Magpie.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class ListRepositoryTests
{
    [TestMethod]
    public void ReturnsFilteredLists()
    {
        var storage = new Mock<IStorage>();
        var gatheringData = new Mock<IGatheringData>();
        var repository = new ListRepository(storage.Object, gatheringData.Object);

        var expectedLists = new List<string>() { "list1", "list2", };
        var returnedLists = new List<string>() { "list1.json", "list2.json", };
        storage.Setup(s => s.GetLists("filter")).Returns(returnedLists);

        CollectionAssert.AreEquivalent(expectedLists, repository.GetLists("filter").ToList());
    }

    [TestMethod]
    public void ReturnsCorrectListByName()
    {
        var storage = new Mock<IStorage>();
        var gatheringData = new Mock<IGatheringData>();
        var repository = new ListRepository(storage.Object, gatheringData.Object);

        var expectedListRecord = new ListRecord() { Name = "testList", Items = [], Abilities = [], };
        storage.Setup(s => s.GetList("testList")).Returns(expectedListRecord);

        Assert.AreEqual(expectedListRecord.Name, repository.GetList("testList").Name);
    }

    [TestMethod]
    public void SavesCorrectList()
    {
        var storage = new Mock<IStorage>();
        var gatheringData = new Mock<IGatheringData>();
        var recipeData = new Mock<IRecipeData>();
        var repository = new ListRepository(storage.Object, gatheringData.Object);

        var listToSave = new GatheringList("savedList", [], []);

        repository.SaveList(listToSave);

        storage.Verify(s => s.SaveList(It.Is<ListRecord>(list => list.Name == "savedList")), Times.Once);
    }

    [TestMethod]
    public void DeletesCorrectList()
    {
        var storage = new Mock<IStorage>();
        var gatheringData = new Mock<IGatheringData>();
        var recipeData = new Mock<IRecipeData>();
        var repository = new ListRepository(storage.Object, gatheringData.Object);

        repository.DeleteList("deleteList");

        storage.Verify(s => s.DeleteList("deleteList"), Times.Once);
    }
}
