namespace Magpie.Extensions;

using Dalamud.Bindings.ImGui;
using static ECommons.ImGuiMethods.ImGuiEx;
using System;
using System.Collections.Generic;

internal static class IEnumerableExtensions
{
    public static IEnumerable<EzTableEntry> ToEzTableEntries<T>(
        this IEnumerable<T> entries,
        string[] columns,
        params Action<T>[] renderers
    )
    {
        var results = new List<EzTableEntry>();
        foreach (var entry in entries) {
            for (var i = 0; i < columns.Length; i++) {
                var column = columns[i];
                var renderer = renderers[i];
                results.Add(new EzTableEntry(column, ImGuiTableColumnFlags.WidthStretch, () => renderer(entry)));
            }
        }
        return results;
    }

}
