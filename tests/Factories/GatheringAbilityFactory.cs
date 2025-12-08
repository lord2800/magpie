namespace MagpieTest.Factories;

using Magpie.Data;

internal static class GatheringAbilityFactory
{
    public static GatheringAbility New(uint id = 1, ushort cost = 1, uint jobId = 1, string jobName = "", string name = "")
        => new(Id: id, Name: name, Cost: cost, JobId: jobId, JobName: jobName);
}
