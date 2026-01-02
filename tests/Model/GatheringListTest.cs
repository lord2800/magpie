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
        var itemToAdd = GatheringListItemFactory.New();
        var gatheringList = new GatheringList("TestList", [], []);

        gatheringList.Add(itemToAdd);

        Assert.AreEqual(1, gatheringList.Items.Count());
        Assert.AreEqual(itemToAdd, gatheringList.Items.First());
    }


    [TestMethod]
    public void UpdatesExistingItemQuantity()
    {
        var itemToAdd = GatheringListItemFactory.New(quantity: 10);
        var gatheringList = new GatheringList("TestList", [], [itemToAdd,]);

        gatheringList.Add(itemToAdd);

        Assert.AreEqual(1, gatheringList.Items.Count());
        Assert.AreEqual(itemToAdd, gatheringList.Items.First());
        Assert.AreEqual(20u, gatheringList.Items.First().Quantity);
    }

    [TestMethod]
    public void RemovesExistingItems()
    {
        var itemToRemove = GatheringListItemFactory.New();
        var gatheringList = new GatheringList("TestList", [], [itemToRemove,]);

        gatheringList.Remove(itemToRemove);

        Assert.AreEqual(0, gatheringList.Items.Count());
    }

    [TestMethod]
    public void IgnoresNonexistentItems()
    {
        var itemToRemove = GatheringListItemFactory.New(1);
        var gatheringList = new GatheringList("TestList", [], [GatheringListItemFactory.New(2),]);

        gatheringList.Remove(itemToRemove);

        Assert.AreEqual(1, gatheringList.Items.Count());
    }


    [TestMethod]
    public void ReducesQuantityWhenListContainsMore()
    {
        var itemToRemove = GatheringListItemFactory.New(itemId: 1, quantity: 2);
        var gatheringList = new GatheringList("TestList", [], [GatheringListItemFactory.New(itemId: 1, quantity: 5),]);

        gatheringList.Remove(itemToRemove);

        Assert.AreEqual(1, gatheringList.Items.Count());
        Assert.AreEqual(3u, gatheringList.Items.First().Quantity);
    }

    [TestMethod]
    public void UpdatesCollectedCountWhenCollecting()
    {
        var item = GatheringListItemFactory.New(itemId: 1, collected: 5);
        var gatheringList = new GatheringList("TestList", [], [item,]);
        var itemToCollect = GatheringListItemFactory.New(itemId: 1, quantity: 1);

        gatheringList.Collect(itemToCollect, 2);

        Assert.AreEqual(7u, gatheringList.Items.First().Collected);
    }


    [TestMethod]
    public void IgnoresNotFoundItemsWhenCollecting()
    {
        var item = GatheringListItemFactory.New(itemId: 1, collected: 5);
        var gatheringList = new GatheringList("TestList", [], [item,]);
        var itemToCollect = GatheringListItemFactory.New(itemId: 2, quantity: 1);

        gatheringList.Collect(itemToCollect, 2);

        Assert.AreEqual(5u, gatheringList.Items.First().Collected);
    }

    [TestMethod]
    public void ResetsCollectedCountForAllItems()
    {
        var gatheringList = new GatheringList("TestList", [], [GatheringListItemFactory.New(collected: 5),]);

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
