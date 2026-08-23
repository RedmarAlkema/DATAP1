using System.Text;
using FsmViewer.Core.Domain;
using FsmViewer.Core.Interfaces;

namespace FsmViewer.Presentation;

public sealed class TextFsmPresenter : IFsmPresenter
{
    public string Present(FiniteStateMachine fsm)
    {
        var output = new StringBuilder();

        output.AppendLine("Finite State Machine");
        output.AppendLine("States:");

        foreach (StateComponent state in fsm.GetStates())
        {
            var visitor = new TextRenderVisitor();
            state.Accept(visitor);
            output.Append(visitor.GetOutput());
        }

        output.AppendLine("Transitions:");

        foreach (Transition transition in fsm.GetTransitions())
        {
            var visitor = new TextRenderVisitor();
            transition.Accept(visitor);
            output.Append(visitor.GetOutput());
        }

        return output.ToString();
    }

    public string Present(StateComponent state)
    {
        var visitor = new TextRenderVisitor();
        state.Accept(visitor);
        var output = new StringBuilder(visitor.GetOutput());

        AppendTransitions("Incoming transitions:", state.GetIncomingTransitions(), output);
        AppendTransitions("Outgoing transitions:", state.GetOutgoingTransitions(), output);

        return output.ToString();
    }

    public string Present(Transition transition)
    {
        var visitor = new TextRenderVisitor();
        transition.Accept(visitor);
        return visitor.GetOutput();
    }

    private static void AppendTransitions(string label, IReadOnlyList<Transition> transitions, StringBuilder output)
    {
        if (transitions.Count == 0)
        {
            return;
        }

        output.AppendLine(label);

        foreach (Transition transition in transitions)
        {
            var visitor = new TextRenderVisitor();
            transition.Accept(visitor);
            output.Append(visitor.GetOutput());
        }
    }
}
