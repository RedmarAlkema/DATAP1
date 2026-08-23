using System.Text;
using FsmViewer.Core.Domain;
using FsmViewer.Core.Interfaces;

namespace FsmViewer.Presentation;

public sealed class TextRenderVisitor : IFsmVisitor
{
    private readonly StringBuilder output = new();
    private int indentationLevel;

    public string GetOutput()
    {
        return output.ToString();
    }

    public void Visit(SimpleState state)
    {
        RenderStateHeader("Simple state", state);
        RenderActions(state);
    }

    public void Visit(CompoundState state)
    {
        RenderStateHeader("Compound state", state);
        RenderActions(state);

        if (state.GetChildren().Count == 0)
        {
            return;
        }

        WriteLine("Children:");
        indentationLevel++;

        foreach (StateComponent child in state.GetChildren())
        {
            child.Accept(this);
        }

        indentationLevel--;
    }

    public void Visit(InitialState state)
    {
        RenderStateHeader("Initial state", state);
        RenderActions(state);
    }

    public void Visit(FinalState state)
    {
        RenderStateHeader("Final state", state);
        RenderActions(state);
    }

    public void Visit(Transition transition)
    {
        string trigger = transition.Trigger is null ? string.Empty : transition.Trigger.Description;
        string guard = transition.HasGuard() ? $" [{transition.Guard}]" : string.Empty;
        string effect = transition.Effect is null ? string.Empty : $" / {transition.Effect.Description}";
        string label = string.IsNullOrEmpty(trigger) ? $"{guard}{effect}" : $"{trigger}{guard}{effect}";

        WriteLine($"Transition {transition.Id}: {transition.Source.Id} --{label}--> {transition.Destination.Id}");
    }

    private void RenderStateHeader(string typeName, StateComponent state)
    {
        string parent = state.Parent is null ? "root" : state.Parent.Id;
        WriteLine($"{typeName} {state.Id}: \"{state.Name}\" (parent: {parent})");
    }

    private void RenderActions(StateComponent state)
    {
        RenderActionsOfType(state, ActionType.ENTRY_ACTION, "On Entry");
        RenderActionsOfType(state, ActionType.DO_ACTION, "Do");
        RenderActionsOfType(state, ActionType.EXIT_ACTION, "On Exit");
    }

    private void RenderActionsOfType(StateComponent state, ActionType type, string label)
    {
        foreach (FsmAction action in state.GetActions().Where(action => action.Type == type))
        {
            WriteLine($"{label}: {action.Description}");
        }
    }

    private void WriteLine(string text)
    {
        output.Append(' ', indentationLevel * 2);
        output.AppendLine(text);
    }
}
