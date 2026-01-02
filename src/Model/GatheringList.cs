namespace Magpie.Model;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Magpie.Data;

public interface IGatheringList
{
    public string Name { get; set; }
    public IEnumerable<GatheringListAbility> Abilities { get; }
    public IEnumerable<GatheringListItem> Items { get; }
    public void Add(GatheringListItem item);
    public void Remove(GatheringListItem item);
    public void Collect(GatheringListItem item, uint quantity);
    public void Reset();
    public void ToggleAbility(GatheringAbility ability, bool? toggle);
    public bool? IsAbilityEnabled(GatheringAbility ability);
}

[ExcludeFromCodeCoverage]
public sealed record GatheringListAbility
{
    public required GatheringAbility Ability;
    public required bool? Enabled;
}

public enum GatheringItemType
{
    Product,
    Gathered,
}

public enum GatheringItemTable
{
    Recipe,
    GatheredItem,
}

[ExcludeFromCodeCoverage]
public sealed record GatheringListItem
{
    public required GatheringItemType Type;
    public required uint ItemId;
    public required uint Quantity { get; set; }
    public required uint Collected { get; set; }
    public static implicit operator string(GatheringListItem item) => $"{item.Type} {item.ItemId}";
}

public sealed class GatheringList(
    string name,
    IEnumerable<GatheringListAbility> abilities,
    IEnumerable<GatheringListItem> items
) : IGatheringList
{
    public string Name { get; set; } = name;
    public IEnumerable<GatheringListAbility> Abilities { get => abilityList; }
    public IEnumerable<GatheringListItem> Items { get => list.Values; }

    private readonly Dictionary<string, GatheringListItem> list = items.ToDictionary(_ => (string)_);
    private readonly List<GatheringListAbility> abilityList = [.. abilities,];

    public void Add(GatheringListItem item)
    {
        list.TryGetValue(item, out var exists);
        if (exists is not null) {
            exists.Quantity += item.Quantity;
        }
        else {
            list.Add(item, item);
        }
    }

    public void Remove(GatheringListItem item)
    {
        list.TryGetValue(item, out var exists);
        if (exists is null) {
            return;
        }

        if (item.Quantity >= exists.Quantity) {
            list.Remove(item);
        }
        else {
            exists.Quantity -= item.Quantity;
        }
    }

    public void Collect(GatheringListItem item, uint quantity)
    {
        list.TryGetValue(item, out var exists);
        if (exists is null) {
            return;
        }

        exists.Collected += quantity;
    }

    public void Reset()
    {
        foreach (var item in Items) {
            item.Collected = 0;
        }
    }

    public void ToggleAbility(GatheringAbility ability, bool? toggle)
    {
        var _ = from x in abilityList where ability.Id == x.Ability.Id select x;
        if (!_.Any()) {
            abilityList.Add(new() { Ability = ability, Enabled = toggle, });
        }
        else {
            _.First().Enabled = toggle;
        }
    }

    public bool? IsAbilityEnabled(GatheringAbility ability)
    {
        return (from x in abilityList where ability == x.Ability select x.Enabled)
            .FirstOrDefault();
    }

    // TODO this needs to be cloned every time it's used
    public static readonly GatheringList Empty = new(string.Empty, [], []) { };
}
