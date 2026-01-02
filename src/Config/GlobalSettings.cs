namespace Magpie.Config;

using Magpie.Data;
using System;
using System.Collections.Generic;

public interface IGlobalSettings : IDisposable
{
    bool EnableAutoretainer { get; set; }
    bool AutoRetainerMultiMode { get; set; }
    bool RepairAtNpc { get; set; }
    bool ShowOverlay { get; set; }

    event Action<bool>? OverlayStateChanged;

    void DisableAbility(GatheringAbility ability);
    void EnableAbility(GatheringAbility ability);
    bool IsAbilityEnabled(GatheringAbility ability);
    void Save();
}

[Autowire(typeof(IGlobalSettings))]
public sealed class GlobalSettings : IGlobalSettings
{
    public bool EnableAutoretainer { get; set; } = false;
    public bool AutoRetainerMultiMode { get; set; } = false;
    public bool RepairAtNpc { get; set; } = false;
    public bool ShowOverlay
    {
        get;
        set {
            field = value;
            OverlayStateChanged?.Invoke(value);
        }
    }

    private readonly List<GatheringAbility> EnabledGatheringListAbilities = [];

    public event Action<bool>? OverlayStateChanged;

    public GlobalSettings() => Load();

    [Shutdown]
    public void Dispose() => Save();

    public bool IsAbilityEnabled(GatheringAbility ability) => EnabledGatheringListAbilities.Contains(ability);
    public void EnableAbility(GatheringAbility ability) => EnabledGatheringListAbilities.Add(ability);
    public void DisableAbility(GatheringAbility ability) => EnabledGatheringListAbilities.Remove(ability);
    private void Load()
    {
        // TODO
    }

    public void Save()
    {
        // TODO
    }
}
