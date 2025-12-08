namespace Magpie.View;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using Magpie.Controller;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

[ExcludeFromCodeCoverage]
[DalamudWindow(typeof(OverlayController)), Autowire]
public sealed class OverlayWindow : Window
{
    private readonly IOverlayController controller;
    private readonly Strings strings;

    public OverlayWindow(IOverlayController controller, Strings strings) : base($"{strings.GetOverlayWindowName()}##overlay")
    {
        this.controller = controller;
        this.strings = strings;
        this.controller.ToggleWindow += (state) => IsOpen = state;

        Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse |
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize;
        SizeCondition = ImGuiCond.Always;
        Size = new Vector2(350, 85);
    }

    public override void Draw()
    {
#pragma warning disable RCS1208
        ImGui.TextUnformatted(strings.GetCurrentGatheringText(controller.List.Name, controller.GetPercentComplete()));
        ImGui.TextUnformatted(strings.GetCurrentItemText(
            controller.GetCurrentItem(),
            controller.GetCurrentItemCount(),
            controller.GetCurrentItemQuantity()
        ));

        ImGui.Separator();
        if (ImGuiEx.IconButton(FontAwesomeIcon.Play, "Start")) {
            controller.Start();
        }
        ImGui.SameLine();
        if (ImGuiEx.IconButton(FontAwesomeIcon.Stop, "Stop")) {
            controller.Stop();
        }
        ImGui.SameLine();
        if (ImGuiEx.IconButton(FontAwesomeIcon.Edit, "Edit")) {
            controller.OpenListEditor();
        }
#pragma warning disable RCS1208
    }
}
