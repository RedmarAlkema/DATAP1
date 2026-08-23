using FsmViewer.Core.Interfaces;

namespace FsmViewer.Core.Domain;

public abstract class StateComponent : IFsmElement
{
    private readonly List<Transition> incomingTransitions = [];
    private readonly List<Transition> outgoingTransitions = [];
    private readonly List<FsmAction> actions = [];

    protected StateComponent(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; }

    public string Name { get; }

    public CompoundState? Parent { get; internal set; }

    public abstract void Accept(IFsmVisitor visitor);

    public void AddIncomingTransition(Transition transition)
    {
        incomingTransitions.Add(transition);
    }

    public void AddOutgoingTransition(Transition transition)
    {
        outgoingTransitions.Add(transition);
    }

    public IReadOnlyList<Transition> GetIncomingTransitions()
    {
        return incomingTransitions;
    }

    public IReadOnlyList<Transition> GetOutgoingTransitions()
    {
        return outgoingTransitions;
    }

    public void AddAction(FsmAction action)
    {
        actions.Add(action);
    }

    public IReadOnlyList<FsmAction> GetActions()
    {
        return actions;
    }
}
