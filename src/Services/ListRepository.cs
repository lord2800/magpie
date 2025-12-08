namespace Magpie.Services;

using Magpie.Data;
using Magpie.Model;
using System.Collections.Generic;
using System.Linq;

public interface IListRepository
{
    void DeleteList(string name);
    IGatheringList GetList(string name);
    IEnumerable<string> GetLists(string filter);
    void SaveList(IGatheringList list);
}

[Autowire(typeof(IListRepository))]
public class ListRepository : IListRepository
{
    private readonly IStorage storage;
    private readonly IGatheringData gathering;

    public ListRepository(IStorage db, IGatheringData gathering)
    {
        this.storage = db;
        this.gathering = gathering;
    }

    public IEnumerable<string> GetLists(string filter)
        // GetFileNameWithoutExtension can return null if the input string is null, which will never be the case here
        => storage.GetLists(filter).Select(System.IO.Path.GetFileNameWithoutExtension)!;

    public IGatheringList GetList(string name) => ToGatheringList(storage.GetList(name));
    public void SaveList(IGatheringList list) => storage.SaveList(ToListRecord(list));
    public void DeleteList(string name) => storage.DeleteList(name);

    private static ListRecord ToListRecord(IGatheringList list)
    {
        return new() {
            Name = list.Name,
            Items = ToListItemRecords(list.Items),
            Abilities = ToListAbilitiesRecords(list.Abilities),
        };
    }

    private static IEnumerable<ListItemRecord> ToListItemRecords(IEnumerable<GatheringListItem> items)
    {
        return from item in items
               select new ListItemRecord() {
                   Type = item.Type,
                   ItemId = item.ItemId,
                   Quantity = item.Quantity,
               };
    }

    private static IEnumerable<ListAbilitiesRecord> ToListAbilitiesRecords(IEnumerable<GatheringListAbility> abilities)
    {
        return from ability in abilities
               select new ListAbilitiesRecord() {
                   AbilityId = ability.Ability.Id,
                   Enabled = ability.Enabled,
               };
    }

    private GatheringList ToGatheringList(ListRecord record)
        => new(record.Name, ToGatheringListAbilities(record.Abilities), ToGatheringListItems(record.Items));

    private static IEnumerable<GatheringListItem> ToGatheringListItems(IEnumerable<ListItemRecord> records)
    {
        return from record in records
               select new GatheringListItem() {
                   Type = record.Type,
                   ItemId = record.ItemId,
                   Quantity = record.Quantity,
                   Collected = record.Collected,
               };
    }

    private IEnumerable<GatheringListAbility> ToGatheringListAbilities(IEnumerable<ListAbilitiesRecord> records)
    {
        return from record in records
               where gathering.Abilities.ContainsKey(record.AbilityId)
               select new GatheringListAbility() {
                   Ability = gathering.Abilities[record.AbilityId],
                   Enabled = record.Enabled,
               };
    }
}
