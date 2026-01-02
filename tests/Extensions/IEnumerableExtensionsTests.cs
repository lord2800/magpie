namespace MagpieTest.Extensions;

using Magpie.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class IEnumerableExtensionsTests
{
    [TestMethod]
    public void EmptyEntriesReturnNoTableEntries()
    {
        var emptyList = new List<TestEntry>();
        var columns = Array.Empty<string>();
        var renderers = Array.Empty<Action<TestEntry>>();

        var result = emptyList.ToEzTableEntries(columns, renderers);

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void SingleColumnSingleEntryCreatesOneTableEntry()
    {
        var entries = new List<TestEntry>() { new("Test1"), };
        var columns = new string[] { "Name", };
        var renderers = new Action<TestEntry>[] { _ => {}, };

        var result = entries.ToEzTableEntries(columns, renderers);

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Name", result.First().ColumnName);
    }

    [TestMethod]
    public void MultipleColumnsAndEntriesCreateCorrectNumberEntries()
    {
        var entries = new List<TestEntry>() { new("Test1"), new("Test2"), };
        var columns = new string[] { "Name", "Value", };
        var renderers = new Action<TestEntry>[] { _ => { }, _ => { }, };

        var result = entries.ToEzTableEntries(columns, renderers);

        Assert.AreEqual(4, result.Count()); // 2 columns * 2 entries
    }

    [TestMethod]
    public void MismatchedColumnsAndRenderersThrowException()
    {
        var entries = new List<TestEntry>() { new("Test"), };
        var columns = new string[] { "Name", };
        var renderers = Array.Empty<Action<TestEntry>>();

        Assert.ThrowsExactly<ArgumentException>(() => entries.ToEzTableEntries(columns, renderers));
    }
}

public class TestEntry(string name)
{
    public string Name { get; set; } = name;
}
