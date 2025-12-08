namespace Magpie.View;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using Magpie.Controller;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

[ExcludeFromCodeCoverage]
[DalamudWindow(typeof(ListSettingsController)), Autowire]
public sealed class ListSettingsWindow : Window
{
    private readonly IListSettingsController controller;
    private readonly Strings strings;

    public ListSettingsWindow(IListSettingsController controller, Strings strings) : base("##list_settings_window")
    {
        this.controller = controller;
        this.strings = strings;
        this.controller.OpenWindow += () => IsOpen = true;
        this.controller.NameUpdated += name => WindowName = $"{strings.GetListSettingsWindowName(name)}##list_settings_{name}";

        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        SizeCondition = ImGuiCond.Always;
        Size = new Vector2(650, 450);
    }

    public override void Draw()
    {
        using (ImRaii.Child("miner_abilities", new Vector2(314, 400), true)) {
            ImGui.TextUnformatted(strings.GetAbilityOptionsLabel(controller.MinerJobName));
            foreach (var ability in controller.MinerAbilities) {
                var useAbility = controller.IsAbilityEnabled(ability);
                ImGuiEx.CheckboxBullet(strings.GetUseAbilityLabel(ability.Name), ref useAbility);
                controller.SetAbilityState(ability, useAbility);
            }
        }

        ImGui.SameLine();

        using (ImRaii.Child("botanist_abilities", new Vector2(314, 400), true)) {
            ImGui.TextUnformatted(strings.GetAbilityOptionsLabel(controller.BotanistJobName));
            foreach (var ability in controller.BotanistAbilities) {
                var useAbility = controller.IsAbilityEnabled(ability);
                ImGuiEx.CheckboxBullet(strings.GetUseAbilityLabel(ability.Name), ref useAbility);
                controller.SetAbilityState(ability, useAbility);
            }
        }
    }
}
