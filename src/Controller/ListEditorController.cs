namespace Magpie.Controller;

using Magpie.Data;
using Magpie.Model;
using Magpie.Services;
using System;
using System.Collections.Generic;

public interface IListEditorController
{
    IGatheringList List { get; }
    IDictionary<uint, Recipe> Recipes { get; }
    IDictionary<uint, GatheringItem> GatheringItems { get; }
    IEnumerable<Ingredient> Gatherables { get; }
    event Action<string>? NameUpdated;
    event Action? OpenWindow;

    public void Open();
    void AddToList(Recipe recipe, uint quantity);
    void AddToList(GatheringItem item, uint quantity);
    void OpenSettings();
    void SaveList();
}

[Autowire(typeof(IListEditorController))]
public class ListEditorController : IListEditorController
{
    private readonly IListSettingsController listSettings;
    private readonly CurrentList list;
    private readonly IRecipeData recipes;
    private readonly IGatheringData gathering;
    private readonly IListRepository repository;

    public ListEditorController(
        IListSettingsController listSettings,
        CurrentList list,
        IRecipeData recipes,
        IGatheringData gathering,
        IListRepository repository
    )
    {
        this.listSettings = listSettings;
        this.list = list;
        this.recipes = recipes;
        this.gathering = gathering;
        this.repository = repository;
        List = list;
        list.NameUpdated += () => NameUpdated?.Invoke(List.Name);
    }

    public IGatheringList List { get; }

    public IDictionary<uint, Recipe> Recipes { get => recipes.Recipes; }
    public IDictionary<uint, GatheringItem> GatheringItems { get => gathering.GatheringItems; }
    public IEnumerable<Ingredient> Gatherables { get => list.Gatherables; }
    public event Action<string>? NameUpdated = null;
    public event Action? OpenWindow = null;

    public void Open() => OpenWindow?.Invoke();

    public void OpenSettings() => listSettings.Open();

    public void AddToList(Recipe recipe, uint quantity)
    {
        list.Add(new GatheringListItem() {
            Type = GatheringItemType.Product,
            ItemId = recipe.Id,
            Quantity = quantity,
            Collected = 0,
        });
    }

    public void AddToList(GatheringItem item, uint quantity)
    {
        list.Add(new GatheringListItem() {
            Type = GatheringItemType.Gathered,
            ItemId = item.Id,
            Quantity = quantity,
            Collected = 0,
        });
    }

    public void SaveList()
    {
        if (!list.IsSaveable) {
            return;
        }

        repository.SaveList(list.List);
    }
}
