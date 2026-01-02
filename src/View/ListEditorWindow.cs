namespace Magpie.View;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using Magpie.Controller;
using Magpie.Extensions;
using Magpie.Model;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

[ExcludeFromCodeCoverage]
[DalamudWindow(typeof(ListEditorController)), Autowire]
public class ListEditorWindow : Window
{
    private const ImGuiTableFlags DEFAULT_FLAGS = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingStretchProp;
    private readonly IListEditorController controller;
    private readonly Strings strings;

    public ListEditorWindow(IListEditorController controller, Strings strings) : base("##list_editor_window")
    {
        this.controller = controller;
        this.strings = strings;
        this.controller.OpenWindow += () => IsOpen = true;
        this.controller.NameUpdated += name => {
            WindowName = name switch {
                "" => $"{strings.GetEmptyListEditorWindowName()}##list_creator",
                _ => $"{strings.GetListEditorWindowName(name)}##list_editor_{name}",
            };
        };

        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        SizeCondition = ImGuiCond.Always;
        SizeConstraints = new WindowSizeConstraints() {
            MinimumSize = new(630, 300),
            MaximumSize = new(630, float.MaxValue),
        };
    }

    public override void OnClose()
    {
        controller.SaveList();
        base.OnClose();
    }

    public override void Draw()
    {
        var rename = controller.List.Name;
        if (ImGui.InputTextWithHint(
            string.Empty,
            strings.GetListNameInputHint(),
            ref rename,
            256,
            ImGuiInputTextFlags.EnterReturnsTrue)
        ) {
            controller.List.Name = rename;
        }

        ImGui.SameLine();
        if (ImGuiEx.IconButton(FontAwesomeIcon.Cog, "Settings")) {
            controller.OpenSettings();
        }

        using (ImRaii.Child("list_table_child", new Vector2(300, 600), true)) {
            var items = controller.List.Items.ToEzTableEntries(
                ["Quantity", "Item",],
                _ => {
                    var qty = _.Quantity;
                    ImGuiEx.InputUint(string.Empty, ref qty);
                    _.Quantity = qty;
                },
                _ => {
                    var name = _.Type switch {
                        GatheringItemType.Product => controller.Recipes[_.ItemId].Name,
                        GatheringItemType.Gathered => controller.GatheringItems[_.ItemId].Name,
                        _ => "<unknown>",
                    };
                    ImGui.TextUnformatted(name);
                }
            );
            var gatherables = controller.Gatherables.ToEzTableEntries(
                ["Quantity", "Item",],
                _ => ImGui.TextUnformatted(_.Amount.ToString()),
                _ => {
                    controller.GatheringItems.TryGetValue(_.Ingredient.TargetId, out var item);
                    ImGui.TextUnformatted(item?.Name ?? "<unknown>");
                }
            );
            var itemTabRenderer = () => ImGuiEx.EzTable("list_editor_table", DEFAULT_FLAGS, items, true);
            var gatherablesTabRenderer = () => ImGuiEx.EzTable("list_editor_table", DEFAULT_FLAGS, gatherables, true);
            ImGuiEx.EzTabBar(
                "list_tables",
                null,
                string.Empty,
                (strings.GetItemTabLabel(), itemTabRenderer, null, true),
                (strings.GetGatherablesTabLabel(), gatherablesTabRenderer, null, true)
            );
        }

        ImGui.SameLine();

        using (ImRaii.Child("item_selector_child", new Vector2(300, 600), true)) {
            const uint quantity = 1;
#pragma warning disable RCS1208
            var recipes = controller.Recipes.Values.ToEzTableEntries(
                ["Recipe",],
                _ => {
                    if (ImGuiUtils.ClickableText(_.Name)) {
                        controller.AddToList(_, quantity);
                    }
                }
            );

            var items = controller.GatheringItems.Values.ToEzTableEntries(
                ["Item",],
                _ => {
                    if (ImGuiUtils.ClickableText(_.Name)) {
                        controller.AddToList(_, quantity);
                    }
                }
            );
#pragma warning restore RCS1208

            var nodeTabRenderer = () => ImGuiEx.EzTable("node_tab", DEFAULT_FLAGS, items, true);
            var recipeTabRenderer = () => ImGuiEx.EzTable("recipe_tab", DEFAULT_FLAGS, recipes, true);

            ImGuiEx.EzTabBar(
                "item_selectors",
                null,
                string.Empty,
                (strings.GetNodeTabLabel(), nodeTabRenderer, null, true),
                (strings.GetRecipeTabLabel(), recipeTabRenderer, null, true)
            );
        }
    }
}
