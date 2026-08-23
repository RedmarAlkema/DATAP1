using FsmViewer.Core.Domain;

namespace FsmViewer.Core.Interfaces;

public interface IFsmVisitor
{
    void Visit(SimpleState state);

    void Visit(CompoundState state);

    void Visit(InitialState state);

    void Visit(FinalState state);

    void Visit(Transition transition);
}
