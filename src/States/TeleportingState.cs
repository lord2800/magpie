namespace Magpie.States;

public class TeleportingState : IStateAware
{
    public string Display => "Teleporting";

    public void Enter() { }

    public void Exit() { }

    public bool IsAllowedFrom(IStateAware state) => true;

    public bool Step() => false;

    public IStateAware[] Transitions() => [];
}
