namespace MagpieTest.Factories;

using Magpie.Data;

internal static class IngredientFactory
{
    public static Ingredient New(
        uint id = 1,
        string name = "",
        IngredientType type = IngredientType.GatheringItem,
        uint targetId = 1,
        uint amount = 1
    )
    {
        return new(Id: id, Type: type, TargetId: targetId, Name: name, Amount: amount);
    }
}
