namespace Magpie.Extensions;

using Dalamud.Bindings.ImGui;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

[ExcludeFromCodeCoverage]
public static class ImGuiUtils
{
    public static bool ClickableText(string text)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.2f, 0.2f, 0.2f, 0.4f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.4f, 0.4f, 0.4f, 0.6f));
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 0.5f, 1.0f, 1.0f));
        var result = false;

        if (ImGui.Button(text)) {
            result = true;
        }

        ImGui.PopStyleColor(4);

        return result;
    }
}
