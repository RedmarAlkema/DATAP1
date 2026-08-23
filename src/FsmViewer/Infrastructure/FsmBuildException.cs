namespace FsmViewer.Infrastructure;

public sealed class FsmBuildException : Exception
{
    public FsmBuildException(string message)
        : base(message)
    {
    }

    public FsmBuildException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
