namespace MagpieTest.Services;

using Magpie.Model;
using Magpie.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

[TestClass]
public class FileSystemStorageTests
{
    private readonly string ListPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    [TestMethod]
    public void CreatesDestinationDirectoryIfNotExistsOnInitialize()
    {
        var fileSystem = new MockFileSystem();
        var serializer = new Mock<JsonSerializer>();
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), serializer.Object);
        fileSystemStorage.Initialize();

        Assert.IsTrue(fileSystem.Directory.Exists(Path.Combine(ListPath, "lists")));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void ReturnsAllListWhenNotProvidedFilter(string filter)
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>() {
            {Path.Combine(ListPath, "lists", "list1.json"), new MockFileData(string.Empty) },
            {Path.Combine(ListPath, "lists", "list2.json"), new MockFileData(string.Empty) },
            {Path.Combine(ListPath, "lists", "filtered.json"), new MockFileData(string.Empty) },
        });
        var serializer = new Mock<JsonSerializer>();
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), serializer.Object);

        var result = fileSystemStorage.GetLists(filter).ToList();

        var expected = new List<string>() { "list1.json", "list2.json", "filtered.json", };
        CollectionAssert.AreEquivalent(expected, result);
    }

    [TestMethod]
    public void ReturnsFilteredListWhenProvidedFilter()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>() {
            {Path.Combine(ListPath, "lists", "list1.json"), new MockFileData(string.Empty) },
            {Path.Combine(ListPath, "lists", "list2.json"), new MockFileData(string.Empty) },
            {Path.Combine(ListPath, "lists", "filtered.json"), new MockFileData(string.Empty) },
        });
        var serializer = new Mock<JsonSerializer>();
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), serializer.Object);

        var result = fileSystemStorage.GetLists("list*").ToList();

        var expected = new List<string>() { "list1.json", "list2.json", };
        CollectionAssert.AreEquivalent(expected, result);
    }

    [TestMethod]
    public void ReturnsListRecordForExistingFile()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>() {
            {Path.Combine(ListPath, "lists", "listName.json"), new MockFileData(@"{""Version"":1,""Name"":""listName"",""Items"":[],""Abilities"":[]}") },
        });
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), new JsonSerializer());

        var result = fileSystemStorage.GetList("listName");

        Assert.AreEqual("listName", result.Name);
    }

    [TestMethod]
    public void ThrowsExceptionForNotBeingAbleToDeserializeList()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>() {
            {Path.Combine(ListPath, "lists", "listName.json"), new MockFileData("null") },
        });
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), new JsonSerializer());

        var exception = Assert.ThrowsExactly<Exception>(() => fileSystemStorage.GetList("listName"));

        Assert.AreEqual("Unable to load list listName", exception.Message);
    }

    [TestMethod]
    public void ThrowsExceptionForMissingNameWhenTryingToRetrieveList()
    {
        var fileSystem = new MockFileSystem();
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), new JsonSerializer());

        var exception = Assert.ThrowsExactly<Exception>(() => fileSystemStorage.GetList(null));
        Assert.AreEqual("Must provide a list name", exception.Message);
    }

    [TestMethod]
    public void ThrowsExceptionForNonExistentFile()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddDirectory(Path.Combine(ListPath, "lists"));
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), new JsonSerializer());

        var exception = Assert.ThrowsExactly<Exception>(() => fileSystemStorage.GetList("nonExistentList"));
        Assert.AreEqual("No list found with name nonExistentList", exception.Message);
    }

    [TestMethod]
    public void SavesListRecordToFile()
    {
        var fileSystem = new MockFileSystem();
        var serializer = new JsonSerializer();
        fileSystem.AddDirectory(Path.Combine(ListPath, "lists"));
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), serializer);

        var listRecord = new ListRecord() { Name = "savedList", Items = [], Abilities = [], };

        fileSystemStorage.SaveList(listRecord);

        var path = Path.Combine(ListPath, "lists", "savedList.json");
        Assert.IsTrue(fileSystem.FileExists(path));
        Assert.AreEqual(@"{""Version"":1,""Name"":""savedList"",""Items"":[],""Abilities"":[]}", fileSystem.File.ReadAllText(path));
    }

    [TestMethod]
    public void OverwritesExistingListWhenSavingToSameFile()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>() {
            { Path.Combine(ListPath, "lists", "savedList.json"), new MockFileData("test") },
        });
        var serializer = new JsonSerializer();
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), serializer);

        var listRecord = new ListRecord() { Name = "savedList", Items = [], Abilities = [], };

        var path = Path.Combine(ListPath, "lists", "savedList.json");
        Assert.AreEqual("test", fileSystem.File.ReadAllText(path));
        fileSystemStorage.SaveList(listRecord);
        Assert.AreEqual(@"{""Version"":1,""Name"":""savedList"",""Items"":[],""Abilities"":[]}", fileSystem.File.ReadAllText(path));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("     ")]
    public void ThrowsExceptionForSavingWithInvalidName(string name)
    {
        var fileSystem = new MockFileSystem();
        var serializer = new Mock<JsonSerializer>();
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), serializer.Object);

        var listRecord = new ListRecord() { Name = name, Items = [], Abilities = [], };

        var exception = Assert.ThrowsExactly<Exception>(() => fileSystemStorage.SaveList(listRecord));
        Assert.AreEqual("Must provide a list name", exception.Message);
    }

    [TestMethod]
    public void DeletesFileForExistingList()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>(){
            { Path.Combine(ListPath, "lists", "listToDelete.json"), new MockFileData(string.Empty) },
        });
        var serializer = new Mock<JsonSerializer>();
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), serializer.Object);

        fileSystemStorage.DeleteList("listToDelete");

        Assert.IsFalse(fileSystem.FileExists(Path.Combine(ListPath, "listslistToDelete.json")));
    }

    [TestMethod]
    public void IgnoresMissingFilesWhenDeletingLists()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddDirectory(Path.Combine(ListPath, "lists"));
        var serializer = new Mock<JsonSerializer>();
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), serializer.Object);

        fileSystemStorage.DeleteList("listToDelete");

        Assert.IsFalse(fileSystem.FileExists(Path.Combine(ListPath, "listslistToDelete.json")));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("     ")]
    public void ThrowsExceptionForDeletingWithInvalidName(string name)
    {
        var fileSystem = new MockFileSystem();
        var serializer = new Mock<JsonSerializer>();
        var fileSystemStorage = new FileSystemStorage(fileSystem, new FileSystemStorageOptions(Path.Combine(ListPath, "lists")), serializer.Object);

        var exception = Assert.ThrowsExactly<Exception>(() => fileSystemStorage.DeleteList(name));
        Assert.AreEqual("Must provide a list name", exception.Message);
    }
}
