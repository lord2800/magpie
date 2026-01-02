namespace Magpie.Controller;

using Magpie.Config;
using Magpie.Data;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IGlobalSettingsController
{
    IEnumerable<string> RequiredPlugins { get; }
    bool IsPluginActive(string plugin);
    bool ShowOverlay { get; set; }
    bool EnableAutoretainer { get; set; }
    bool MultiMode { get; set; }
    bool RepairAtNpc { get; set; }
    IEnumerable<GatheringAbility> GatheringAbilities { get; }
    IEnumerable<GatheringAbility> MinerAbilities { get; }
    IEnumerable<GatheringAbility> BotanistAbilities { get; }
    string MinerJobName { get; }
    string BotanistJobName { get; }
    public event Action? ToggleWindow;

    void Toggle();
    bool IsAbilityEnabled(GatheringAbility ability);
    void SetAbilityState(GatheringAbility ability, bool enabled);
    void Save();
}

[Autowire(typeof(IGlobalSettingsController))]
public class GlobalSettingsController(
    IGlobalSettings settings,
    IActivePluginState activePlugins,
    IGatheringData abilities
) : IGlobalSettingsController
{
    public IEnumerable<string> RequiredPlugins { get => activePlugins.RequiredPlugins; }
    public bool IsPluginActive(string plugin) => activePlugins.ActivePlugins.Contains(plugin);

    public bool ShowOverlay
    {
        get => settings.ShowOverlay;
        set => settings.ShowOverlay = value;
    }

    public bool EnableAutoretainer
    {
        get => settings.EnableAutoretainer;
        set => settings.EnableAutoretainer = value;
    }

    public bool MultiMode
    {
        get => settings.AutoRetainerMultiMode;
        set => settings.AutoRetainerMultiMode = value;
    }

    public bool RepairAtNpc
    {
        get => settings.RepairAtNpc;
        set => settings.RepairAtNpc = value;
    }

    public event Action? ToggleWindow = null;
    public void Toggle() => ToggleWindow?.Invoke();

    public IEnumerable<GatheringAbility> GatheringAbilities => abilities.Abilities.Values;
    public IEnumerable<GatheringAbility> MinerAbilities => GetAbilitiesByJob(abilities.MinerJobId);
    public IEnumerable<GatheringAbility> BotanistAbilities => GetAbilitiesByJob(abilities.BotanistJobId);

    public string MinerJobName { get => abilities.MinerJobName; }
    public string BotanistJobName { get => abilities.BotanistJobName; }

    private IEnumerable<GatheringAbility> GetAbilitiesByJob(uint id) =>
        from ability in GatheringAbilities where ability.JobId == id select ability;

    public bool IsAbilityEnabled(GatheringAbility ability) => settings.IsAbilityEnabled(ability);

    public void SetAbilityState(GatheringAbility ability, bool enabled)
    {
        if (enabled) {
            settings.EnableAbility(ability);
        }
        else {
            settings.DisableAbility(ability);
        }
    }

    public void Save() => settings.Save();
}
