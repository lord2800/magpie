namespace Magpie;

using Dalamud.Game.ClientState.Objects;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using Magpie.Extensions;
using Magpie.Services;
using Magpie.View;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;

[ExcludeFromCodeCoverage]
public sealed class Plugin : IDalamudPlugin
{
    private readonly IServiceProvider container;
    public const string Name = "Magpie";
    public static readonly string Version = typeof(Plugin).Assembly.GetName().Version!.ToString(2);

#pragma warning disable CA2211 // Only used for debugging
    public static IPluginLog? StaticLogger;
#pragma warning restore CA2211

    public Plugin(
        IPluginLog logger,
        IDalamudPluginInterface pluginInterface,
        IClientState clientState,
        IFramework framework,
        IObjectTable objectTable,
        ITargetManager targetManager,
        IGameInventory gameInventory,
        IDataManager dataManager
    )
    {
        StaticLogger = logger;
        ECommonsMain.Init(pluginInterface, this);

        logger.Info("Booting container");
        container = BuildContainer(
            logger,
            pluginInterface,
            clientState,
            framework,
            objectTable,
            targetManager,
            gameInventory,
            dataManager
        );

        logger.Info("Initializing window system");
        var windowSystem = container.Get<WindowSystem>();
        RegisterUI(logger, pluginInterface, windowSystem);
        InitializeWindows(logger);

        logger.Info("Initializing all initializers");
        RunInitializers(logger);

        logger.Info("Ready to collect shinies");
    }

    private void InitializeWindows(IPluginLog logger)
    {
        logger.Verbose("Registering windows");
        var windowSystem = container.Get<WindowSystem>();
        foreach (var type in FindTypesByAttribute<DalamudWindowAttribute>(Assembly.GetExecutingAssembly().GetTypes())) {
#if DEBUG
            logger.Verbose($"Registering window {type.DeclaringType}");
#endif
            windowSystem.AddWindow((Window)container.Get(type.DeclaringType));
        }
    }

    private void RunInitializers(IPluginLog logger)
    {
        foreach (var method in FindMethodsByAttribute<InitializerAttribute>(Assembly.GetExecutingAssembly().GetTypes())) {
#if DEBUG
            logger.Verbose($"Calling {method.DeclaringType}.{method.DeclaringMethod.Name}()");
#endif
            method.DeclaringMethod.Invoke(container.Get(method.DeclaringType), null);
        }
    }

    private static ServiceProvider BuildContainer(
        IPluginLog logger,
        IDalamudPluginInterface pluginInterface,
        IClientState clientState,
        IFramework framework,
        IObjectTable objectTable,
        ITargetManager targetManager,
        IGameInventory gameInventory,
        IDataManager dataManager
    )
    {
        var collection = new ServiceCollection();

        #region Dalamud services
        logger.Verbose("Registering Dalamud services");
        collection
            .AddSingleton(typeof(IPluginLog), logger)
            .AddSingleton(typeof(IDalamudPluginInterface), pluginInterface)
            .AddSingleton(typeof(IClientState), clientState)
            .AddSingleton(typeof(IFramework), framework)
            .AddSingleton(typeof(IObjectTable), objectTable)
            .AddSingleton(typeof(ITargetManager), targetManager)
            .AddSingleton(typeof(IGameInventory), gameInventory)
            .AddSingleton(typeof(IDataManager), dataManager)
            .AddSingleton(new WindowSystem(Name))
        ;
        #endregion

        #region Stuff not autowireable
        logger.Verbose("Registering database components");
        collection
            .AddSingleton(new FileSystemStorageOptions(Path.Join(pluginInterface.GetPluginConfigDirectory(), "lists")))
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton(JsonSerializer.CreateDefault())
        ;
        #endregion

        RegisterAutowired(logger, collection);

        return collection.BuildServiceProvider(
#if DEBUG
            new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true, }
#endif
        );
    }

    private static void RegisterAutowired(IPluginLog logger, ServiceCollection collection)
    {
        logger.Verbose("Registering all autowired classes");
        foreach (var type in FindTypesByAttribute<AutowireAttribute>(Assembly.GetExecutingAssembly().GetTypes())) {
            var registrationType = type.Attribute.SubstituteType ?? type.DeclaringType;
#if DEBUG
            logger.Verbose($"Registering {type.DeclaringType} as {registrationType}");
#endif

            collection.AddSingleton(registrationType, type.DeclaringType);

            if (registrationType != type.DeclaringType) {
#if DEBUG
                logger.Verbose($"Registering {type.DeclaringType} under {registrationType}");
#endif
                collection.AddSingleton(type.DeclaringType, _ => _.Get(registrationType));
            }
        }
    }

    private static IEnumerable<AttributedType<T>> FindTypesByAttribute<T>(IEnumerable<Type> values) where T : Attribute
    {
        return from v in values
               where v.GetCustomAttribute<T>(true) is not null
               select new AttributedType<T>(v, v.GetCustomAttribute<T>(true)!);
    }

    private static IEnumerable<AttributedMethod<T>> FindMethodsByAttribute<T>(IEnumerable<Type> values) where T : Attribute
    {
        return from v in values
               from m in v.GetMethods()
               where m.GetCustomAttribute<T>(true) is not null
               select new AttributedMethod<T>(m, v, m.GetCustomAttribute<T>(true)!);
    }

    private void ToggleMain() => container.Get<MainWindow>().Toggle();
    private void ToggleConfig() => container.Get<GlobalSettingsWindow>().Toggle();

    private void RegisterUI(IPluginLog logger, IDalamudPluginInterface pluginInterface, WindowSystem windowSystem)
    {
        logger.Debug("Registering windowing system");
        pluginInterface.UiBuilder.Draw += windowSystem.Draw;
        pluginInterface.UiBuilder.OpenMainUi += ToggleMain;
        pluginInterface.UiBuilder.OpenConfigUi += ToggleConfig;
    }

    private void DeregisterUI()
    {
        var windowSystem = container.Get<WindowSystem>();
        var pluginInterface = container.Get<IDalamudPluginInterface>();

        pluginInterface.UiBuilder.Draw -= windowSystem.Draw;
        pluginInterface.UiBuilder.OpenMainUi -= ToggleMain;
        pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfig;
        windowSystem.RemoveAllWindows();
    }

    public void Dispose()
    {
        ECommonsMain.Dispose();
        DeregisterUI();

        var logger = container.Get<IPluginLog>();
        foreach (var method in FindMethodsByAttribute<ShutdownAttribute>(Assembly.GetExecutingAssembly().GetTypes())) {
#if DEBUG
            logger.Verbose($"Calling {method.DeclaringType}.{method.DeclaringMethod.Name}()");
#endif
            method.DeclaringMethod.Invoke(container.Get(method.DeclaringType), null);
        }

        logger.Info("Shinies collected! Shutting down now. Bye!");
    }
}

[ExcludeFromCodeCoverage]
internal record AttributedType<T>(Type DeclaringType, T Attribute) where T : Attribute;

[ExcludeFromCodeCoverage]
internal record AttributedMethod<T>(MethodInfo DeclaringMethod, Type DeclaringType, T Attribute) where T : Attribute;
