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
        EnsureUniqueId(id);

        StateCreator creator = stateCreators[type];
        StateComponent state = creator.Create(id, name);
        stateIndex[id] = state;

        if (parentId == "_")
        {
            result.AddState(state);
            return;
        }

        if (!stateIndex.TryGetValue(parentId, out StateComponent? parent))
        {
            throw new FsmBuildException($"State '{id}' references unknown parent state '{parentId}'.");
        }

        if (parent is not CompoundState compoundState)
        {
            throw new FsmBuildException($"State '{id}' references parent '{parentId}', but that parent is not a CompoundState.");
        }

        compoundState.Add(state);
    }

    public void BuildTrigger(string id, string description)
    {
        EnsureUniqueId(id);

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

        if (!stateIndex.TryGetValue(ownerId, out StateComponent? state))
        {
            throw new FsmBuildException($"Action references unknown state owner '{ownerId}'.");
        }

        state.AddAction(action);
    }

    public void BuildTransition(string id, string sourceId, string destinationId, string? triggerId, string? guard)
    {
        if (!stateIndex.TryGetValue(sourceId, out StateComponent? source))
        {
            throw new FsmBuildException($"Transition '{id}' references unknown source state '{sourceId}'.");
        }

        if (!stateIndex.TryGetValue(destinationId, out StateComponent? destination))
        {
            throw new FsmBuildException($"Transition '{id}' references unknown destination state '{destinationId}'.");
        }

        FsmTrigger? trigger = null;
        if (triggerId is not null && !triggerIndex.TryGetValue(triggerId, out trigger))
        {
            throw new FsmBuildException($"Transition '{id}' references unknown trigger '{triggerId}'.");
        }

        FsmAction? effect = pendingTransitionActions.GetValueOrDefault(id);
        var transition = new Transition(id, source, destination, trigger, guard, effect);

        result.AddTransition(transition);
        pendingTransitionActions.Remove(id);
    }

    public FiniteStateMachine GetResult()
    {
        if (pendingTransitionActions.Count > 0)
        {
            string ownerIds = string.Join(", ", pendingTransitionActions.Keys.Order());
            throw new FsmBuildException($"Transition action references unknown transition owner(s): {ownerIds}.");
        }

        return result;
    }

    private void EnsureUniqueId(string id)
    {
        if (stateIndex.ContainsKey(id) || triggerIndex.ContainsKey(id))
        {
            throw new FsmBuildException($"Identifier '{id}' is already defined.");
        }
    }
}
