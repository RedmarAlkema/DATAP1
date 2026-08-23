using FsmViewer.Core.Domain;

namespace FsmViewer.Core.Validators;

public sealed class DeterminismValidator : FsmValidator
{
    protected override void ValidateStates(FiniteStateMachine fsm)
    {
        foreach (StateComponent state in fsm.GetAllStates())
        {
            IReadOnlyList<Transition> outgoing = state.GetOutgoingTransitions();

            if (outgoing.Count <= 1)
            {
                continue;
            }

            if (outgoing.Any(transition => transition.IsAutomatic() && !transition.HasGuard()))
            {
                AddError("Unguarded automatic transition cannot be combined with other outgoing transitions.", state.Id);
            }

            foreach (IGrouping<string, Transition> guardGroup in outgoing
                         .Where(transition => transition.IsAutomatic())
                         .GroupBy(transition => NormalizeGuard(transition.Guard)))
            {
                if (guardGroup.Count() > 1)
                {
                    AddError($"Multiple automatic transitions use guard '{guardGroup.Key}'.", state.Id);
                }
            }

            foreach (IGrouping<string, Transition> triggerGroup in outgoing
                         .Where(transition => !transition.IsAutomatic())
                         .GroupBy(transition => transition.Trigger!.Id))
            {
                List<Transition> transitions = triggerGroup.ToList();

                if (transitions.Count <= 1)
                {
                    continue;
                }

                if (transitions.Any(transition => string.IsNullOrWhiteSpace(transition.Guard)))
                {
                    AddError($"Multiple outgoing transitions use trigger '{triggerGroup.Key}' without distinguishing guards.", state.Id);
                }

                foreach (IGrouping<string, Transition> guardGroup in transitions.GroupBy(transition => NormalizeGuard(transition.Guard)))
                {
                    if (guardGroup.Count() > 1)
                    {
                        AddError($"Multiple outgoing transitions use trigger '{triggerGroup.Key}' and guard '{guardGroup.Key}'.", state.Id);
                    }
                }
            }
        }
    }

    private static string NormalizeGuard(string? guard)
    {
        return string.IsNullOrWhiteSpace(guard) ? string.Empty : guard.Trim();
    }
}
