namespace Magpie.Services.IPC;

using System;
using System.Diagnostics.CodeAnalysis;
using ECommons.EzIpcManager;

public interface ILifestreamClient
{
    bool AethernetTeleport(string destination);
    bool Teleport(uint destination, byte subIndex);
    bool TeleportToHome();
    bool TeleportToFC();
    bool TeleportToApartment();
    bool IsBusy();
    void ExecuteCommand(string args);
}

[ExcludeFromCodeCoverage]
[Autowire]
public class LifestreamIpc
{
    private const string Service = "Lifestream";

    [Initializer]
    public void Initialize() => EzIPC.Init(this, Service);

#pragma warning disable CS8618 // Initialized as part of EzIPC
    [EzIPC] public Func<string, bool> AethernetTeleport;
    [EzIPC] public Func<uint, byte, bool> Teleport;
    [EzIPC] public Func<bool> TeleportToHome;
    [EzIPC] public Func<bool> TeleportToFC;
    [EzIPC] public Func<bool> TeleportToApartment;
    [EzIPC] public Func<bool> IsBusy;
    [EzIPC] public Action<string> ExecuteCommand;
#pragma warning restore CS8618
}

[Autowire(typeof(ILifestreamClient))]
public class LifestreamClient(LifestreamIpc ipc) : ILifestreamClient
{
    public bool AethernetTeleport(string destination) => ipc.AethernetTeleport(destination);
    public bool Teleport(uint destination, byte subIndex) => ipc.Teleport(destination, subIndex);
    public bool TeleportToHome() => ipc.TeleportToHome();
    public bool TeleportToFC() => ipc.TeleportToFC();
    public bool TeleportToApartment() => ipc.TeleportToApartment();
    public bool IsBusy() => ipc.IsBusy();
    public void ExecuteCommand(string args) => ipc.ExecuteCommand(args);
}
