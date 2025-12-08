namespace Magpie.Services.IPC;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using ECommons.EzIpcManager;

public interface IVnavClient
{
    bool PathfindInProgress();
    bool PathfindAndMoveCloseTo(Vector3 dest, bool fly, float range);
    public bool IsReady();
    public bool IsRunning();
    public float BuildProgress();
    public bool Reload();
    public bool Rebuild();
}

[ExcludeFromCodeCoverage]
[Autowire]
public class VnavIpc
{
    private const string Service = "vnavmesh";

    [Initializer]
    public void Initialize() => EzIPC.Init(this, Service);

#pragma warning disable CS8618 // Initialized as part of EzIPC
    [EzIPC("SimpleMove.%m")] public Func<Vector3, bool, float, bool> PathfindAndMoveCloseTo;
    [EzIPC("SimpleMove.%m")] public Func<bool> PathfindInProgress;
    [EzIPC("Nav.%m")] public Func<bool> IsReady;
    [EzIPC("Path.%m")] public Func<bool> IsRunning;
    [EzIPC("Nav.%m")] public Func<float> BuildProgress;
    [EzIPC("Nav.%m")] public Func<bool> Reload;
    [EzIPC("Nav.%m")] public Func<bool> Rebuild;
#pragma warning restore CS8618
}

[Autowire(typeof(IVnavClient))]
public sealed class VnavClient(VnavIpc ipc) : IVnavClient
{
    public bool PathfindAndMoveCloseTo(Vector3 dest, bool fly, float range) => ipc.PathfindAndMoveCloseTo(dest, fly, range);
    public bool PathfindInProgress() => ipc.PathfindInProgress();
    public bool IsReady() => ipc.IsReady();
    public bool IsRunning() => ipc.IsRunning();
    public float BuildProgress() => ipc.BuildProgress();
    public bool Reload() => ipc.Reload();
    public bool Rebuild() => ipc.Rebuild();
}
