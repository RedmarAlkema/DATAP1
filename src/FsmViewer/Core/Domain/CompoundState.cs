using FsmViewer.Core.Interfaces;

namespace FsmViewer.Core.Domain;

public sealed class CompoundState : StateComponent
{
    private readonly List<StateComponent> children = [];

    public CompoundState(string id, string name)
        : base(id, name)
    {
    }

    public void Add(StateComponent state)
    {
        state.Parent = this;
        children.Add(state);
    }

    public void Remove(StateComponent state)
    {
        if (children.Remove(state))
        {
            state.Parent = null;
        }
    }

    public IReadOnlyList<StateComponent> GetChildren()
    {
        return children;
    }

    public override void Accept(IFsmVisitor visitor)
    {
        visitor.Visit(this);
    }
}
