namespace Magpie.Controller;

using Magpie.Data;
using Magpie.Services;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IListSettingsController
{
    IEnumerable<GatheringAbility> GatheringAbilities { get; }
    IEnumerable<GatheringAbility> MinerAbilities { get; }
    IEnumerable<GatheringAbility> BotanistAbilities { get; }
    string MinerJobName { get; }
    string BotanistJobName { get; }

    event Action<string>? NameUpdated;
    event Action? OpenWindow;

    bool IsAbilityEnabled(GatheringAbility ability);
    void Open();
    void SetAbilityState(GatheringAbility ability, bool? enabled);
}

[Autowire(typeof(IListSettingsController))]
public class ListSettingsController : IListSettingsController
{
    private readonly CurrentList list;
    private readonly IGatheringData gathering;

    public ListSettingsController(
        CurrentList list,
        IGatheringData gathering
    )
    {
        this.list = list;
        this.gathering = gathering;
        this.list.NameUpdated += () => NameUpdated?.Invoke(list.Name);
    }

    public event Action<string>? NameUpdated = null;
    public event Action? OpenWindow = null;

    public IEnumerable<GatheringAbility> GatheringAbilities => gathering.Abilities.Values;
    public IEnumerable<GatheringAbility> MinerAbilities => GetAbilitiesByJob(gathering.MinerJobId);
    public IEnumerable<GatheringAbility> BotanistAbilities => GetAbilitiesByJob(gathering.BotanistJobId);

    public string MinerJobName { get => gathering.MinerJobName; }
    public string BotanistJobName { get => gathering.BotanistJobName; }

    private IEnumerable<GatheringAbility> GetAbilitiesByJob(uint id) =>
        from ability in GatheringAbilities where ability.JobId == id select ability;

    public bool IsAbilityEnabled(GatheringAbility ability) => list.IsAbilityEnabled(ability) ?? false;

    public void SetAbilityState(GatheringAbility ability, bool? enabled) => list.ToggleAbility(ability, enabled);

    public void Open() => OpenWindow?.Invoke();
}
