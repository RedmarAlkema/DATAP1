using FsmViewer.Core.Domain;

namespace FsmViewer.Infrastructure;

public sealed class FsmLoader
{
    public FiniteStateMachine Load(string filePath)
    {
        var parser = new FsmParser();
        var builder = new FsmBuilder();
        var director = new FsmDirector(builder);

        List<FsmToken> tokens = parser.Parse(filePath);
        director.Construct(tokens);

        return builder.GetResult();
    }
}
