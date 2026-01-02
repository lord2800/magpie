namespace Magpie.States;

using Magpie.Services.IPC;

[Autowire]
public sealed class MovingState(IVnavClient client) : IStateAware
{
    public void Enter() { }

    public void Exit() { }

    public bool IsAllowedFrom(IStateAware state)
        => state is PathfindingState;

    public bool Step() => !client.IsRunning();

    public IStateAware[] Transitions()
        => throw new System.NotImplementedException();

    public string Display => "Moving";
}
