using System.Text.RegularExpressions;

namespace FsmViewer.Infrastructure;

public sealed partial class FsmParser
{
    public List<FsmToken> Parse(string filePath)
    {
        var tokens = new List<FsmToken>();

        int lineNumber = 0;

        foreach (string line in File.ReadLines(filePath))
        {
            lineNumber++;
            string trimmed = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
            {
                continue;
            }

            tokens.Add(ParseLine(trimmed, lineNumber));
        }

        return tokens;
    }

    private static FsmToken ParseLine(string line, int lineNumber)
    {
        Match stateMatch = StateRegex().Match(line);
        if (stateMatch.Success)
        {
            return new FsmToken(LineType.STATE, [
                stateMatch.Groups["id"].Value,
                stateMatch.Groups["parent"].Value,
                stateMatch.Groups["name"].Value,
                stateMatch.Groups["type"].Value
            ], lineNumber);
        }

        Match triggerMatch = TriggerRegex().Match(line);
        if (triggerMatch.Success)
        {
            return new FsmToken(LineType.TRIGGER, [
                triggerMatch.Groups["id"].Value,
                triggerMatch.Groups["description"].Value
            ], lineNumber);
        }

        Match actionMatch = ActionRegex().Match(line);
        if (actionMatch.Success)
        {
            return new FsmToken(LineType.ACTION, [
                actionMatch.Groups["owner"].Value,
                actionMatch.Groups["description"].Value,
                actionMatch.Groups["type"].Value
            ], lineNumber);
        }

        Match transitionMatch = TransitionRegex().Match(line);
        if (transitionMatch.Success)
        {
            return ParseTransition(transitionMatch, lineNumber);
        }

        throw new FormatException($"Line {lineNumber}: could not parse FSM line: {line}");
    }

    private static FsmToken ParseTransition(Match match, int lineNumber)
    {
        string id = match.Groups["id"].Value;
        string source = match.Groups["source"].Value;
        string destination = match.Groups["destination"].Value;
        string remainder = match.Groups["remainder"].Value.Trim();
        string triggerId = string.Empty;
        string guard = string.Empty;

        if (!string.IsNullOrEmpty(remainder))
        {
            if (remainder.StartsWith('"'))
            {
                guard = ExtractQuotedValue(remainder);
            }
            else
            {
                int firstSpace = remainder.IndexOf(' ');
                if (firstSpace < 0)
                {
                    triggerId = remainder;
                }
                else
                {
                    triggerId = remainder[..firstSpace];
                    string rest = remainder[firstSpace..].Trim();

                    if (!string.IsNullOrEmpty(rest))
                    {
                        guard = ExtractQuotedValue(rest);
                    }
                }
            }
        }

        return new FsmToken(LineType.TRANSITION, [id, source, destination, triggerId, guard], lineNumber);
    }

    private static string ExtractQuotedValue(string value)
    {
        Match match = QuotedValueRegex().Match(value);
        if (!match.Success)
        {
            throw new FormatException($"Expected quoted value in '{value}'.");
        }

        return match.Groups["value"].Value;
    }

    [GeneratedRegex("^STATE\\s+(?<id>\\S+)\\s+(?<parent>\\S+)\\s+\"(?<name>[^\"]*)\"\\s*:\\s*(?<type>INITIAL|SIMPLE|COMPOUND|FINAL)\\s*;$")]
    private static partial Regex StateRegex();

    [GeneratedRegex("^TRIGGER\\s+(?<id>\\S+)\\s+\"(?<description>[^\"]*)\"\\s*;$")]
    private static partial Regex TriggerRegex();

    [GeneratedRegex("^ACTION\\s+(?<owner>\\S+)\\s+\"(?<description>[^\"]*)\"\\s*:\\s*(?<type>ENTRY_ACTION|DO_ACTION|EXIT_ACTION|TRANSITION_ACTION)\\s*;$")]
    private static partial Regex ActionRegex();

    [GeneratedRegex("^TRANSITION\\s+(?<id>\\S+)\\s+(?<source>\\S+)\\s*->\\s*(?<destination>\\S+)\\s*(?<remainder>.*?)\\s*;$")]
    private static partial Regex TransitionRegex();

    [GeneratedRegex("^\"(?<value>[^\"]*)\"$")]
    private static partial Regex QuotedValueRegex();
}
