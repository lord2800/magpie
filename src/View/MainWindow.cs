namespace Magpie.View;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using Magpie.Controller;
using Magpie.Extensions;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
[DalamudWindow(typeof(MainController)), Autowire]
internal sealed class MainWindow : Window
{
    private readonly IMainController controller;
    private readonly Strings strings;

    public MainWindow(IMainController controller, Strings strings) : base($"{strings.GetMainWindowName()}##main_window")
    {
        this.controller = controller;
        this.strings = strings;

        Flags = ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        SizeCondition = ImGuiCond.Always;
        SizeConstraints = new WindowSizeConstraints() {
            MinimumSize = new(350, 165),
            MaximumSize = new(600, float.MaxValue),
        };

        TitleBarButtons.Add(new TitleBarButton() {
            Icon = FontAwesomeIcon.Cog,
            Priority = int.MinValue,
            IconOffset = new(1.5f, 1),
            Click = _ => controller.ToggleConfigWindow(),
            ShowTooltip = () => Tooltip(strings.GetConfigCogTooltip()),
        });
    }

    private static void Tooltip(string text)
    {
        ImGui.BeginTooltip();
        ImGui.Text(text);
        ImGui.EndTooltip();
    }

    public override void Draw()
    {
        ImGui.Text(strings.GetMainWindowHeaderText());

        ImGui.SameLine();
        ImGui.AlignTextToFramePadding();
        if (ImGuiEx.IconButton(FontAwesomeIcon.Plus, "Add")) {
            controller.OpenListEditor(null);
        }
        ImGui.SameLine();
        if (ImGuiEx.IconButton(FontAwesomeIcon.FileImport, "Import")) {
            controller.OpenImport();
        }

        // TODO stateful start/stop buttons depending on list selection

        var filter = string.Empty;
        ImGui.InputTextWithHint(string.Empty, strings.GetSearchListsTextHint(), ref filter);
        ImGui.Separator();

        var lists = controller.GetLists(filter);
        ImGuiEx.EzTable("list_table", lists.ToEzTableEntries(
            ["List", "Actions",],
            _ => ImGui.TextUnformatted(_),
            _ => {
#pragma warning disable RCS1208
                if (ImGuiEx.IconButton(FontAwesomeIcon.Edit, "Edit")) {
                    controller.OpenListEditor(_);
                }
                ImGui.SameLine();
                if (ImGuiEx.IconButton(FontAwesomeIcon.Trash, "Delete")) {
                    controller.DeleteList(_);
                }
#pragma warning restore RCS1208
            }
        ));
    }
}
