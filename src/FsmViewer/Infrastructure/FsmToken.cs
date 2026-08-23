namespace FsmViewer.Infrastructure;

public sealed class FsmToken
{
    public FsmToken(LineType type, IReadOnlyList<string> tokens, int lineNumber)
    {
        Type = type;
        Tokens = tokens;
        LineNumber = lineNumber;
    }

    public LineType Type { get; }

    public IReadOnlyList<string> Tokens { get; }

    public int LineNumber { get; }
}
