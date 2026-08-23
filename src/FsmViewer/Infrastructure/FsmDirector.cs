using FsmViewer.Core.Domain;
using FsmViewer.Core.Interfaces;

namespace FsmViewer.Infrastructure;

public sealed class FsmDirector
{
    private readonly IFsmBuilder builder;

    public FsmDirector(IFsmBuilder builder)
    {
        this.builder = builder;
    }

    public void Construct(IReadOnlyList<FsmToken> tokens)
    {
        builder.Reset();

        foreach (FsmToken token in tokens)
        {
            try
            {
                switch (token.Type)
                {
                    case LineType.STATE:
                        builder.BuildState(
                            token.Tokens[0],
                            token.Tokens[1],
                            token.Tokens[2],
                            Enum.Parse<StateType>(token.Tokens[3]));
                        break;
                    case LineType.TRIGGER:
                        builder.BuildTrigger(token.Tokens[0], token.Tokens[1]);
                        break;
                    case LineType.ACTION:
                        builder.BuildAction(
                            token.Tokens[0],
                            token.Tokens[1],
                            Enum.Parse<ActionType>(token.Tokens[2]));
                        break;
                    case LineType.TRANSITION:
                        builder.BuildTransition(
                            token.Tokens[0],
                            token.Tokens[1],
                            token.Tokens[2],
                            EmptyToNull(token.Tokens[3]),
                            EmptyToNull(token.Tokens[4]));
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported line type '{token.Type}'.");
                }
            }
            catch (FsmBuildException exception)
            {
                throw new FsmBuildException($"Line {token.LineNumber}: {exception.Message}", exception);
            }
        }
    }

    private static string? EmptyToNull(string value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }
}
