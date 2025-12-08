namespace MagpieTest.Factories;

using Magpie.Data;
using Magpie.Model;

internal static class GatheringListAbilityFactory
{
    public static GatheringListAbility New(GatheringAbility ability, bool enabled = true) => new() { Ability = ability, Enabled = enabled, };
}
