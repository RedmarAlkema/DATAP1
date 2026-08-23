using FsmViewer.Core.Interfaces;

namespace FsmViewer.Core.Domain;

public sealed class SimpleState : StateComponent
{
    public SimpleState(string id, string name)
        : base(id, name)
    {
    }

    public override void Accept(IFsmVisitor visitor)
    {
        visitor.Visit(this);
    }
}
