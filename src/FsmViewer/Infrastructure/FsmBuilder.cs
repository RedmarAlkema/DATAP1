using FsmViewer.Core.Domain;
using FsmViewer.Core.Interfaces;

namespace FsmViewer.Infrastructure;

public sealed class FsmBuilder : IFsmBuilder
{
    private readonly Dictionary<StateType, StateCreator> stateCreators;
    private FiniteStateMachine result = new();
    private Dictionary<string, StateComponent> stateIndex = [];
    private Dictionary<string, FsmTrigger> triggerIndex = [];
    private Dictionary<string, FsmAction> pendingTransitionActions = [];

    public FsmBuilder()
        : this(new Dictionary<StateType, StateCreator>
        {
            [StateType.INITIAL] = new InitialStateCreator(),
            [StateType.SIMPLE] = new SimpleStateCreator(),
            [StateType.COMPOUND] = new CompoundStateCreator(),
            [StateType.FINAL] = new FinalStateCreator()
        })
    {
    }

    public FsmBuilder(Dictionary<StateType, StateCreator> stateCreators)
    {
        this.stateCreators = stateCreators;
    }

    public void Reset()
    {
        result = new FiniteStateMachine();
        stateIndex = [];
        triggerIndex = [];
        pendingTransitionActions = [];
    }

    public void BuildState(string id, string parentId, string name, StateType type)
    {
        StateCreator creator = stateCreators[type];
        StateComponent state = creator.Create(id, name);
        stateIndex[id] = state;

        if (parentId != "_" && stateIndex.TryGetValue(parentId, out StateComponent? parent) && parent is CompoundState compoundState)
        {
            compoundState.Add(state);
            return;
        }

        result.AddState(state);
    }

    public void BuildTrigger(string id, string description)
    {
        var trigger = new FsmTrigger(id, description);
        triggerIndex[id] = trigger;
        result.AddTrigger(trigger);
    }

    public void BuildAction(string ownerId, string description, ActionType type)
    {
        var action = new FsmAction(ownerId, description, type);
        result.AddAction(action);

        if (type == ActionType.TRANSITION_ACTION)
        {
            pendingTransitionActions[ownerId] = action;
            return;
        }

        if (stateIndex.TryGetValue(ownerId, out StateComponent? state))
        {
            state.AddAction(action);
        }
    }

    public void BuildTransition(string id, string sourceId, string destinationId, string? triggerId, string? guard)
    {
        StateComponent source = stateIndex[sourceId];
        StateComponent destination = stateIndex[destinationId];
        FsmTrigger? trigger = triggerId is null ? null : triggerIndex.GetValueOrDefault(triggerId);
        FsmAction? effect = pendingTransitionActions.GetValueOrDefault(id);
        var transition = new Transition(id, source, destination, trigger, guard, effect);

        result.AddTransition(transition);
    }

    public FiniteStateMachine GetResult()
    {
        return result;
    }
}
