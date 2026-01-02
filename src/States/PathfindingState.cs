namespace Magpie.States;

using Magpie.Services.IPC;

[Autowire]
public sealed class PathfindingState(MovingState moving, IVnavClient client) : IStateAware
{
    public void Enter()
    {
        // TODO call vnav pathfindandmovecloseto but how to get the data for where to go?
    }

    public void Exit() { }

    public bool IsAllowedFrom(IStateAware state)
        => throw new System.NotImplementedException();

    public bool Step() => !client.PathfindInProgress();

    public IStateAware[] Transitions() => (client.PathfindInProgress()) ? [] : [moving,];

    public string Display => "Pathfinding";
}
