namespace Magpie.Extensions;

using Microsoft.Extensions.DependencyInjection;
using System;

public static class DependencyInjectorExtensions
{
    public static T Get<T>(this IServiceProvider _) where T : class => _.GetRequiredService<T>();
    public static object Get(this IServiceProvider _, Type t) => _.GetRequiredService(t);
}
