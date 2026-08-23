using FsmViewer.Core.Domain;

namespace FsmViewer.Core.Validators;

public sealed class CompoundTransitionValidator : FsmValidator
{
    protected override void ValidateTransitions(FiniteStateMachine fsm)
    {
        foreach (Transition transition in fsm.GetTransitions())
        {
            if (transition.Destination is CompoundState)
            {
                AddError("Transition cannot end at a CompoundState.", transition.Id);
            }
        }
    }
}
