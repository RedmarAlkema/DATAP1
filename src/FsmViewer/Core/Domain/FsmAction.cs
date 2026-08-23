namespace FsmViewer.Core.Domain;

public sealed class FsmAction
{
    public FsmAction(string ownerId, string description, ActionType type)
    {
        OwnerId = ownerId;
        Description = description;
        Type = type;
    }

    public string OwnerId { get; }

    public string Description { get; }

    public ActionType Type { get; }
}
