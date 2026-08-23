namespace FsmViewer.Core.Domain;

public sealed class FiniteStateMachine
{
    private readonly List<StateComponent> states = [];
    private readonly List<Transition> transitions = [];
    private readonly List<FsmTrigger> triggers = [];
    private readonly List<FsmAction> actions = [];

    public void AddState(StateComponent state)
    {
        states.Add(state);
    }

    public void AddTransition(Transition transition)
    {
        transitions.Add(transition);
        transition.Source.AddOutgoingTransition(transition);
        transition.Destination.AddIncomingTransition(transition);
    }

    public void AddTrigger(FsmTrigger trigger)
    {
        triggers.Add(trigger);
    }

    public void AddAction(FsmAction action)
    {
        actions.Add(action);
    }

    public IReadOnlyList<StateComponent> GetStates()
    {
        return states;
    }

    public IReadOnlyList<StateComponent> GetAllStates()
    {
        var allStates = new List<StateComponent>();

        foreach (StateComponent state in states)
        {
            AddStateAndChildren(state, allStates);
        }

        return allStates;
    }

    public IReadOnlyList<Transition> GetTransitions()
    {
        return transitions;
    }

    public IReadOnlyList<FsmTrigger> GetTriggers()
    {
        return triggers;
    }

    public IReadOnlyList<FsmAction> GetActions()
    {
        return actions;
    }

    public StateComponent? GetState(string id)
    {
        return GetAllStates().FirstOrDefault(state => state.Id == id);
    }

    public Transition? GetTransition(string id)
    {
        return transitions.FirstOrDefault(transition => transition.Id == id);
    }

    public FsmTrigger? GetTrigger(string id)
    {
        return triggers.FirstOrDefault(trigger => trigger.Id == id);
    }

    private static void AddStateAndChildren(StateComponent state, List<StateComponent> allStates)
    {
        allStates.Add(state);

        if (state is not CompoundState compoundState)
        {
            return;
        }

        foreach (StateComponent child in compoundState.GetChildren())
        {
            AddStateAndChildren(child, allStates);
        }
    }
}
