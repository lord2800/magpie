namespace Magpie.Extensions;

using Lumina.Text.ReadOnly;
using System;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class SeStringExtensions
{
    public static bool EqualsIgnoreCase(this in ReadOnlySeString seString, string compare)
        => seString.ExtractText().Equals(compare, StringComparison.InvariantCultureIgnoreCase);
}
