namespace Magpie.Services;

using Magpie.Config;
using Magpie.Data;
using Magpie.Model;
using System;
using System.Collections.Generic;
using System.Linq;

[Autowire]
public class CurrentList(IGatheringData gathering, IRecipeData recipe, IGlobalSettings settings) : IGatheringList
{
    public IGatheringList List { get; set; } = GatheringList.Empty;

    public string Name
    {
        get => List.Name;
        set {
            List.Name = value;
            NameUpdated?.Invoke();
        }
    }
    public event Action? NameUpdated;
    public IEnumerable<GatheringListAbility> Abilities { get => List.Abilities; }
    public IEnumerable<GatheringListItem> Items { get => List.Items; }
    public bool IsSaveable { get => !string.IsNullOrEmpty(Name) && List.Items.Any(); }

    public IEnumerable<Ingredient> Gatherables
    {
        get {
            var results = new Dictionary<uint, Ingredient>();
            foreach (var item in Items) {
                if (item.Type == GatheringItemType.Gathered) {
                    var ingredient = ConvertItemToIngredient(item);
                    if (results.TryGetValue(ingredient.TargetId, out var result)) {
                        results[ingredient.TargetId] = result with { Amount = result.Amount + ingredient.Amount };
                    }
                    else {
                        results.Add(ingredient.TargetId, ingredient);
                    }
                }
                else if (item.Type == GatheringItemType.Product) {
                    foreach (var ingredient in recipe.GetAllIngredients(recipe.Recipes[item.ItemId])) {
                        if (results.TryGetValue(ingredient.TargetId, out var result)) {
                            results[ingredient.TargetId] = result with { Amount = result.Amount + item.Quantity };
                        }
                        else {
                            results.Add(ingredient.TargetId, ingredient with { Amount = ingredient.Amount * item.Quantity });
                        }
                    }
                }
            }
            return results.Values;
        }
    }
    private Ingredient ConvertItemToIngredient(GatheringListItem item)
    {
        var gatheringItem = gathering.GatheringItems[item.ItemId];
        return new(
            Id: gatheringItem.Id,
            Type: IngredientType.GatheringItem,
            TargetId: gatheringItem.ItemId,
            Name: gatheringItem.Name,
            Amount: item.Quantity
        );
    }

    public void Add(GatheringListItem item) => List.Add(item);
    public bool? IsAbilityEnabled(GatheringAbility ability)
        => List.IsAbilityEnabled(ability) ?? settings.IsAbilityEnabled(ability);
    public void Remove(GatheringListItem item) => List.Remove(item);
    public void Collect(GatheringListItem item, uint quantity) => List.Collect(item, quantity);
    public void Reset() => List.Reset();
    public void ToggleAbility(GatheringAbility ability, bool? toggle) => List.ToggleAbility(ability, toggle);
}
