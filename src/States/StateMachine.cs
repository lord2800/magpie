namespace Magpie.States;

public interface IStateAware
{
    public string Display { get; }
    public void Enter();
    public bool Step();
    public void Exit();
    public IStateAware[] Transitions();
    public bool IsAllowedFrom(IStateAware state);
}

public interface IStateMachine
{
    void Start();
    void Step();
    void Stop();
}

[Autowire(typeof(IStateMachine))]
public class StateMachine(Start start, Stop stop) : IStateMachine
{
    private IStateAware state = start;
    public void Step()
    {
        if (!state.Step()) {
            return;
        }

        foreach (var transition in state.Transitions()) {
            if (transition.IsAllowedFrom(state)) {
                TransitionState(transition);
            }
        }
    }

    public void Start() => TransitionState(start);
    public void Stop() => TransitionState(stop);

    private void TransitionState(IStateAware transition)
    {
        state.Exit();
        transition.Enter();
        state = transition;
    }
}

[Autowire]
public class Start(PathfindingState pathfinding) : IStateAware
{
    public void Enter() { }

    public void Exit() { }

    public bool IsAllowedFrom(IStateAware state) => true;

    public bool Step() => true;

    public IStateAware[] Transitions() => [pathfinding,];

    public string Display => "Ready to start";
}

[Autowire]
public class Stop(Start start) : IStateAware
{
    public void Enter() { }

    public void Exit() { }

    public bool IsAllowedFrom(IStateAware state) => true;

    public bool Step() => false;

    public IStateAware[] Transitions() => [start,];

    public string Display => "Stopping";
}
