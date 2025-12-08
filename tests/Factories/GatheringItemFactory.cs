namespace MagpieTest.Factories;

using Magpie.Data;

internal static class GatheringItemFactory
{
    public static GatheringItem New(uint id = 1, uint itemId = 1, string name = "", bool hidden = false)
        => new(Id: id, ItemId: itemId, Name: name, Hidden: hidden);
}
