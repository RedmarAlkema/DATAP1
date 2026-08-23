namespace FsmViewer.Core.Validators;

public sealed class ValidationError
{
    public ValidationError(string message, string? elementId = null)
    {
        Message = message;
        ElementId = elementId;
    }

    public string Message { get; }

    public string? ElementId { get; }

    public override string ToString()
    {
        return ElementId is null ? Message : $"{ElementId}: {Message}";
    }
}
