using FsmViewer.Core.Interfaces;

namespace FsmViewer.Core.Domain;

public sealed class FinalState : StateComponent
{
    public FinalState(string id, string name)
        : base(id, name)
    {
    }

    public override void Accept(IFsmVisitor visitor)
    {
        visitor.Visit(this);
    }
}
