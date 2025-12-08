namespace Magpie.Data;

using ActionSheet = Lumina.Excel.Sheets.Action;
using Dalamud.Plugin.Services;
using GatheringItemSheet = Lumina.Excel.Sheets.GatheringItem;
using ItemSheet = Lumina.Excel.Sheets.Item;
using Lumina.Excel.Sheets;
using Magpie.Extensions;
using System.Collections.Generic;
using System.Linq;

public interface IGatheringData
{
    public string MinerJobName { get; }
    public string MinerJobAbbreviation { get; }
    public uint MinerJobId { get; }
    public string BotanistJobName { get; }
    public string BotanistJobAbbreviation { get; }
    public uint BotanistJobId { get; }
    public IDictionary<uint, GatheringAbility> Abilities { get; }
    public IDictionary<uint, GatheringItem> GatheringItems { get; }
}

[Autowire(typeof(IGatheringData))]
public class GatheringData(IDataManager dataManager) : IGatheringData
{
    private readonly ClassJob MinerJob = (from job in dataManager.GetExcelSheet<ClassJob>()
                                          where job.Abbreviation.EqualsIgnoreCase("min")
                                          select job).FirstOrDefault();

    public string MinerJobName { get => MinerJob.Name.ExtractText(); }
    public string MinerJobAbbreviation { get => MinerJob.Abbreviation.ExtractText(); }
    public uint MinerJobId { get => MinerJob.RowId; }

    private readonly ClassJob BotanistJob = (from job in dataManager.GetExcelSheet<ClassJob>()
                                             where job.Abbreviation.EqualsIgnoreCase("btn")
                                             select job).FirstOrDefault();

    public string BotanistJobName { get => BotanistJob.Name.ExtractText(); }
    public string BotanistJobAbbreviation { get => BotanistJob.Abbreviation.ExtractText(); }
    public uint BotanistJobId { get => BotanistJob.RowId; }

    private readonly IDictionary<uint, GatheringAbility> abilities = (from ability in dataManager.GetExcelSheet<ActionSheet>()
                                                                      where
                                                                          ability.ActionCategory.IsValid && ability.ActionCategory.Value.Name.EqualsIgnoreCase("dol ability") &&
                                                                          ability.AnimationEnd.IsValid && (
                                                                              ability.AnimationEnd.Value.Key.EqualsIgnoreCase("gather/buff") ||
                                                                              ability.AnimationEnd.Value.Key.EqualsIgnoreCase("gather/libura")
                                                                          ) &&
                                                                          ability.ClassJob.IsValid && (
                                                                              ability.ClassJob.Value.Abbreviation.EqualsIgnoreCase("min") ||
                                                                              ability.ClassJob.Value.Abbreviation.EqualsIgnoreCase("btn")
                                                                          )
                                                                      orderby ability.ClassJob.RowId
                                                                      select new GatheringAbility(
                                                                          Id: ability.RowId,
                                                                          Name: ability.Name.ExtractText(),
                                                                          Cost: ability.PrimaryCostValue,
                                                                          JobId: ability.ClassJob.RowId,
                                                                          JobName: ability.ClassJob.Value.Name.ExtractText()
                                                                      )).ToDictionary(_ => _.Id);

    public readonly IDictionary<uint, GatheringItem> gatheringItems = (from item in dataManager.GetExcelSheet<GatheringItemSheet>()
                                                                       where
                                                                           item.Unknown4 // Unknown4 seems to be a flag for the item being active
                                                                       select new GatheringItem(
                                                                           Id: item.RowId,
                                                                           ItemId: item.Item.RowId,
                                                                           // the lumina sheet seems to be wrong, it doesn't turn item.Item into a RowRef<ItemSheet>,
                                                                           // instead just a plain RowRef
                                                                           Name: (from _ in dataManager.GetExcelSheet<ItemSheet>() where _.RowId == item.Item.RowId select _)
                                                                               .FirstOrDefault().Name.ExtractText(),
                                                                           Hidden: item.IsHidden
                                                                       )).ToDictionary(_ => _.Id);

#pragma warning disable RCS1085
    // this must be like this in order to force the recipe data to be cached
    // while still being a correct interface implementation
    public IDictionary<uint, GatheringAbility> Abilities => abilities;
    public IDictionary<uint, GatheringItem> GatheringItems => gatheringItems;
#pragma warning restore RCS1085

}

public record GatheringItem(uint Id, uint ItemId, string Name, bool Hidden);

public record GatheringAbility(uint Id, string Name, ushort Cost, uint JobId, string JobName);
