using FsmViewer.Core.Domain;

namespace FsmViewer.Core.Interfaces;

public interface IFsmPresenter
{
    string Present(FiniteStateMachine fsm);

    string Present(StateComponent state);

    string Present(Transition transition);
}
