namespace Magpie.Extensions;

using Dalamud.Plugin.Services;
using Lumina.Excel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class DependencyInjectorExtensions
{
    public static T Get<T>(this IServiceProvider _) where T : class => _.GetRequiredService<T>();
    public static object Get(this IServiceProvider _, Type t) => _.GetRequiredService(t);
    public static IServiceCollection AddExcelTable<T>(
        this IServiceCollection collection,
        IDataManager dataManager,
        IPluginLog logger
    ) where T : struct, IExcelRow<T>
    {
        logger.Verbose($"Registering data table {typeof(T)}");
        return collection.AddSingleton<IReadOnlyCollection<T>>(_ => dataManager.GetExcelSheet<T>());
    }
}
