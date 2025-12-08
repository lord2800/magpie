namespace MagpieTest.Factories;

using Magpie.Model;

internal static class GatheringListItemFactory
{
    public static GatheringListItem New(
        uint itemId = 1,
        GatheringItemType type = GatheringItemType.Product,
        uint quantity = 1,
        uint collected = 1
    )
    {
        return new() { Type = type, Collected = collected, ItemId = itemId, Quantity = quantity, };
    }
}
