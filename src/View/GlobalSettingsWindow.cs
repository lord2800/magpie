namespace Magpie.View;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using Magpie.Controller;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

[ExcludeFromCodeCoverage]
[DalamudWindow(typeof(GlobalSettingsController)), Autowire]
public sealed class GlobalSettingsWindow : Window
{
    private readonly IGlobalSettingsController controller;
    private readonly Strings strings;

    private Vector4 GetPluginActiveColor(string plugin)
        => (controller.IsPluginActive(plugin)) ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed;

    private FontAwesomeIcon GetPluginActiveIcon(string plugin)
        => (controller.IsPluginActive(plugin)) ? FontAwesomeIcon.Check : FontAwesomeIcon.Times;

    public GlobalSettingsWindow(IGlobalSettingsController controller, Strings strings) : base($"{strings.GetGlobalSettingsWindowName()}##global_settings")
    {
        this.controller = controller;
        this.strings = strings;
        controller.ToggleWindow += Toggle;

        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        Size = new Vector2(650, 570);
        SizeCondition = ImGuiCond.Always;
    }

    public override void Draw()
    {
        foreach (var plugin in controller.RequiredPlugins) {
            using (ImRaii.PushColor(ImGuiCol.Text, GetPluginActiveColor(plugin))) {
                ImGuiComponents.IconButtonWithText(GetPluginActiveIcon(plugin), strings.GetPluginInstalledText(plugin));
            }
        }

        bool showOverlay = controller.ShowOverlay,
             enableAutoretainer = controller.EnableAutoretainer,
             multiMode = controller.MultiMode,
             repairAtNpc = controller.RepairAtNpc;

        ImGuiEx.CheckboxBullet(strings.GetShowOverlayText(), ref showOverlay);
        ImGuiEx.CheckboxBullet(strings.GetEnableAutoRetainerText(), ref enableAutoretainer);
        ImGuiEx.CheckboxBullet(strings.GetMultiModeText(), ref multiMode);
        ImGuiEx.CheckboxBullet(strings.GetRepairAtNPCText(), ref repairAtNpc);

        controller.ShowOverlay = showOverlay;
        controller.EnableAutoretainer = enableAutoretainer;
        controller.MultiMode = multiMode;
        controller.RepairAtNpc = repairAtNpc;

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

    public override void OnClose()
    {
        base.OnClose();
        controller.Save();
    }
}
