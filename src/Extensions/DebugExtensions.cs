using System.Diagnostics.CodeAnalysis;

namespace Magpie.Extensions;

[ExcludeFromCodeCoverage]
public static class DebugExtensions
{
#pragma warning disable RCS1175
    public static void DebugBacktrace(this object obj)
#pragma warning restore RCS1175
    {
        Plugin.StaticLogger?.Verbose(System.Environment.StackTrace);
    }
}
