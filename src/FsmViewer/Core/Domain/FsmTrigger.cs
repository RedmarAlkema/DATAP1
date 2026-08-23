namespace FsmViewer.Core.Domain;

public sealed class FsmTrigger
{
    public FsmTrigger(string id, string description)
    {
        Id = id;
        Description = description;
    }

    public string Id { get; }

    public string Description { get; }
}
