using FsmViewer.Core.Interfaces;

namespace FsmViewer.Core.Domain;

public sealed class Transition : IFsmElement
{
    public Transition(
        string id,
        StateComponent source,
        StateComponent destination,
        FsmTrigger? trigger,
        string? guard,
        FsmAction? effect)
    {
        Id = id;
        Source = source;
        Destination = destination;
        Trigger = trigger;
        Guard = guard;
        Effect = effect;
    }

    public string Id { get; }

    public StateComponent Source { get; }

    public StateComponent Destination { get; }

    public FsmTrigger? Trigger { get; }

    public string? Guard { get; }

    public FsmAction? Effect { get; }

    public void Accept(IFsmVisitor visitor)
    {
        visitor.Visit(this);
    }

    public bool IsAutomatic()
    {
        return Trigger is null;
    }

    public bool HasGuard()
    {
        return !string.IsNullOrWhiteSpace(Guard);
    }
}
