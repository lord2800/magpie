namespace MagpieTest.Model;

using Magpie.Model;
using MagpieTest.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public sealed class GatheringListTests
{
    [TestMethod]
    public void AssignsPropertiesCorrectly()
    {
        var abilities = new List<GatheringListAbility>() {
            GatheringListAbilityFactory.New(GatheringAbilityFactory.New()),
        };
        var items = new List<GatheringListItem>() {
            GatheringListItemFactory.New(),
        };
        var gatheringList = new GatheringList("TestList", abilities, items);
        Assert.AreEqual("TestList", gatheringList.Name);
        Assert.AreEqual(abilities.Count, gatheringList.Abilities.Count());
        Assert.AreEqual(items.Count, gatheringList.Items.Count());
    }

    [TestMethod]
    public void UpdatesName()
    {
        var gatheringList = new GatheringList("TestList", [], []);

        Assert.AreEqual("TestList", gatheringList.Name);

        gatheringList.Name = "Updated";

        Assert.AreEqual("Updated", gatheringList.Name);
    }

    [TestMethod]
    public void AddsNewItems()
    {
        var itemToAdd = new GatheringListItem() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, };
        var gatheringList = new GatheringList("TestList", [], []);

        gatheringList.Add(itemToAdd);

        Assert.AreEqual(1, gatheringList.Items.Count());
        Assert.AreEqual(itemToAdd, gatheringList.Items.First());
    }


    [TestMethod]
    public void UpdatesExistingItemQuantity()
    {
        var itemToAdd = new GatheringListItem() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, };
        var gatheringList = new GatheringList("TestList", [], [itemToAdd,]);

        gatheringList.Add(itemToAdd);

        Assert.AreEqual(1, gatheringList.Items.Count());
        Assert.AreEqual(itemToAdd, gatheringList.Items.First());
        Assert.AreEqual(20u, gatheringList.Items.First().Quantity);
    }

    [TestMethod]
    public void RemovesExistingItems()
    {
        var gatheringList = new GatheringList("TestList", [], [new() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, },]);
        var itemToRemove = new GatheringListItem() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, };

        gatheringList.Remove(itemToRemove);

        Assert.AreEqual(0, gatheringList.Items.Count());
    }

    [TestMethod]
    public void IgnoresNonexistentItems()
    {
        var itemToRemove = new GatheringListItem() { Type = GatheringItemType.Product, ItemId = 2, Quantity = 10, Collected = 5, };
        var gatheringList = new GatheringList("TestList", [], [new() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, },]);

        gatheringList.Remove(itemToRemove);

        Assert.AreEqual(1, gatheringList.Items.Count());
    }


    [TestMethod]
    public void ReducesQuantityWhenListContainsMore()
    {
        var itemToRemove = new GatheringListItem() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 7, Collected = 5, };
        var gatheringList = new GatheringList("TestList", [], [new() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, },]);

        gatheringList.Remove(itemToRemove);

        Assert.AreEqual(1, gatheringList.Items.Count());
        Assert.AreEqual(3u, gatheringList.Items.First().Quantity);
    }

    [TestMethod]
    public void UpdatesCollectedCountWhenCollecting()
    {
        var gatheringList = new GatheringList("TestList", [], [new() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, },]);
        var itemToCollect = new GatheringListItem() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, };

        gatheringList.Collect(itemToCollect, 2);

        Assert.AreEqual(7u, gatheringList.Items.First().Collected);
    }


    [TestMethod]
    public void IgnoresMissingItemsWhenCollecting()
    {
        var gatheringList = new GatheringList("TestList", [], [new() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, },]);
        var itemToCollect = new GatheringListItem() { Type = GatheringItemType.Product, ItemId = 2, Quantity = 10, Collected = 5, };

        gatheringList.Collect(itemToCollect, 2);

        Assert.AreEqual(5u, gatheringList.Items.First().Collected);
    }

    [TestMethod]
    public void ResetsCollectedCountForAllItems()
    {
        var gatheringList = new GatheringList("TestList", [], [new() { Type = GatheringItemType.Product, ItemId = 1, Quantity = 10, Collected = 5, },]);

        Assert.AreEqual(5u, gatheringList.Items.First().Collected);

        gatheringList.Reset();

        Assert.AreEqual(0u, gatheringList.Items.First().Collected);
    }

    [TestMethod]
    public void TogglesAbilityStateWhenPresentWithoutAdding()
    {
        var ability = GatheringListAbilityFactory.New(GatheringAbilityFactory.New());
        var gatheringList = new GatheringList("TestList", [ability,], []);

        gatheringList.ToggleAbility(ability.Ability, false);
        Assert.AreEqual(1, gatheringList.Abilities.Count());
        Assert.IsFalse(gatheringList.IsAbilityEnabled(ability.Ability));

        gatheringList.ToggleAbility(ability.Ability, true);
        Assert.AreEqual(1, gatheringList.Abilities.Count());
        Assert.IsTrue(gatheringList.IsAbilityEnabled(ability.Ability));
    }


    [TestMethod]
    public void AddsAbilityToListWhenToggledAndNotPresent()
    {
        var ability = GatheringListAbilityFactory.New(GatheringAbilityFactory.New());
        var gatheringList = new GatheringList("TestList", [], []);
        Assert.AreEqual(0, gatheringList.Abilities.Count());

        gatheringList.ToggleAbility(ability.Ability, false);
        Assert.AreEqual(1, gatheringList.Abilities.Count());
        Assert.IsFalse(gatheringList.IsAbilityEnabled(ability.Ability));

        gatheringList.ToggleAbility(ability.Ability, true);
        Assert.AreEqual(1, gatheringList.Abilities.Count());
        Assert.IsTrue(gatheringList.IsAbilityEnabled(ability.Ability));
    }

    [TestMethod]
    public void ReturnsAbilityEnabledState()
    {
        var ability = GatheringListAbilityFactory.New(GatheringAbilityFactory.New());
        var gatheringList = new GatheringList("TestList", [ability,], []);
        Assert.IsTrue(gatheringList.IsAbilityEnabled(ability.Ability));
    }
}
