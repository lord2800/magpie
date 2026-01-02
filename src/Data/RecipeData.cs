namespace Magpie.Data;

using Dalamud.Plugin.Services;
using ItemSheet = Lumina.Excel.Sheets.Item;
using GatheringItemSheet = Lumina.Excel.Sheets.GatheringItem;
using RecipeSheet = Lumina.Excel.Sheets.Recipe;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Lumina.Extensions;
using System.Collections.Frozen;

public interface IRecipeData
{
    public FrozenDictionary<uint, Recipe> Recipes { get; }
    public IEnumerable<Ingredient> GetAllIngredients(Recipe recipe);
}

[Autowire(typeof(IRecipeData))]
public class RecipeData(
    IReadOnlyCollection<RecipeSheet> recipeData,
    IReadOnlyCollection<GatheringItemSheet> gatheringItemData,
    IReadOnlyCollection<ItemSheet> itemData,
    IPluginLog logger
) : IRecipeData
{
    private readonly FrozenDictionary<uint, Recipe> recipes =
        (from recipe in recipeData
         where recipe.ItemResult.IsValid && recipe.AmountResult > 0
         select new Recipe(
             Id: recipe.RowId,
             Name: recipe.ItemResult.Value.Name.ExtractText(),
             Ingredients: BuildRecipeIngredients(recipe)
        )).ToDictionary(x => x.Id).ToFrozenDictionary();

    private static List<RecipeIngredient> BuildRecipeIngredients(in RecipeSheet recipe)
    {
        var results = new List<RecipeIngredient>();

        for (var i = 0; i < recipe.Ingredient.Count; i++) {
            var ingredient = recipe.Ingredient[i];

            if (!ingredient.IsValid || recipe.AmountIngredient[i] < 1) {
                continue;
            }

            results.Add(new RecipeIngredient(
                Id: ingredient.RowId,
                Name: ingredient.Value.Name.ExtractText(),
                Amount: recipe.AmountIngredient[i]
            ));
        }

        return results;
    }

#pragma warning disable RCS1085
    // this must be like this in order to force the recipe data to be cached
    // while still being a correct interface implementation
    public FrozenDictionary<uint, Recipe> Recipes => recipes;
#pragma warning restore RCS1085

    private readonly ConcurrentDictionary<uint, IEnumerable<Ingredient>> cache = [];

    public IEnumerable<Ingredient> GetAllIngredients(Recipe recipe)
    {
        if (cache.TryGetValue(recipe.Id, out var cached)) {
            return cached;
        }

        var result = new List<Ingredient>();
        var ingredients = recipe.Ingredients;

        logger.Verbose($"Recipe for {recipe.Name} ({recipe.Id}) not cached, expanding ingredient list");
        while (ingredients.Count > 0) {
            var ingredient = ingredients[0];
            ingredients.Remove(ingredient);

            logger.Verbose($"Examining ingredient {ingredient.Name} ({ingredient.Id})");
            var gatherable = from _ in gatheringItemData
                             where _.Item.RowId == ingredient.Id && _.Unknown4
                             select _;
            var isGatherable = gatherable.Any();

            if (!isGatherable) {
                result = [.. result, .. ExpandRecipeIngredient(ingredient),];
                continue;
            }

            result.Add(new Ingredient(
                Id: ingredient.Id,
                Type: IngredientType.GatheringItem,
                TargetId: gatherable.First().RowId,
                Name: ingredient.Name,
                Amount: ingredient.Amount
            ));
        }

        cache[recipe.Id] = result;

        return result;
    }

    private IEnumerable<Ingredient> ExpandRecipeIngredient(RecipeIngredient ingredient)
    {
        var item = (from _ in itemData
            where _.RowId == ingredient.Id
            select _).FirstOrNull();
        if (item is null) {
            return [];
        }
        var recipeId = (from _ in recipeData
            where _.ItemResult.RowId == item.Value.RowId
            select _.RowId).FirstOrNull();
        if (recipeId is null) {
            return [];
        }
        return GetAllIngredients(recipes[recipeId.Value]);
    }
}

public record Recipe(uint Id, string Name, List<RecipeIngredient> Ingredients);

public record RecipeIngredient(uint Id, string Name, uint Amount);

public enum IngredientType
{
    Recipe,
    GatheringItem,
}
public record Ingredient(uint Id, IngredientType Type, uint TargetId, string Name, uint Amount);
