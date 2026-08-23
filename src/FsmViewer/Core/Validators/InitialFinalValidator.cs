using FsmViewer.Core.Domain;

namespace FsmViewer.Core.Validators;

public sealed class InitialFinalValidator : FsmValidator
{
    protected override void ValidateStates(FiniteStateMachine fsm)
    {
        foreach (StateComponent state in fsm.GetAllStates())
        {
            if (state is InitialState && state.GetIncomingTransitions().Count > 0)
            {
                AddError("InitialState cannot have incoming transitions.", state.Id);
            }

            if (state is FinalState && state.GetOutgoingTransitions().Count > 0)
            {
                AddError("FinalState cannot have outgoing transitions.", state.Id);
            }
        }
    }
}
