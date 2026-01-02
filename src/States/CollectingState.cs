namespace Magpie.States;

public sealed class CollectingState : IStateAware
{
    public void Enter()
        => throw new System.NotImplementedException();

    public void Exit()
        => throw new System.NotImplementedException();

    public bool IsAllowedFrom(IStateAware state)
        => throw new System.NotImplementedException();

    public bool Step()
        => throw new System.NotImplementedException();

    public IStateAware[] Transitions()
        => throw new System.NotImplementedException();

    public string Display => "Collecting from node";
}
