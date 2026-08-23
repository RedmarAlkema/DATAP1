namespace FsmViewer.Infrastructure;

public sealed class FsmToken
{
    public FsmToken(LineType type, IReadOnlyList<string> tokens)
    {
        Type = type;
        Tokens = tokens;
    }

    public LineType Type { get; }

    public IReadOnlyList<string> Tokens { get; }
}
