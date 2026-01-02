namespace Magpie.Services;

using System;
using Dalamud.Plugin.Services;
using Magpie.States;

public interface IOrchestrator
{
    Gatherable CurrentItem { get; }

    void Dispose();
    void Initialize();
}

// TODO come up with a better name for this
[Autowire(typeof(IOrchestrator))]
public class Orchestrator(
    IFramework framework,
    IStateMachine stateMachine,
    CurrentList list
) : IDisposable, IOrchestrator
{
    [Initializer]
    public void Initialize()
    {
        stateMachine.Start();
        framework.Update += OnUpdate;
    }

    [Shutdown]
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        stateMachine.Stop();
        framework.Update -= OnUpdate;
    }

    private void OnUpdate(IFramework framework) => stateMachine.Step();

    public Gatherable CurrentItem
    {
        get {
            return null;
        }
    }
}
