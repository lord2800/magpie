namespace Magpie.Model;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed record ListRecord
{
    public int Version = 1;
    public required string Name;
    public required IEnumerable<ListItemRecord> Items;
    public required IEnumerable<ListAbilitiesRecord> Abilities;
}

[ExcludeFromCodeCoverage]
public sealed record ListItemRecord
{
    public required GatheringItemType Type;
    public required uint ItemId;
    public required uint Quantity;
    public uint Collected;
}

[ExcludeFromCodeCoverage]
public class ListAbilitiesRecord
{
    public required uint AbilityId;
    public required bool? Enabled;
}
