using FsmViewer.Core.Domain;

namespace FsmViewer.Core.Interfaces;

public interface IFsmBuilder
{
    void Reset();

    void BuildState(string id, string parentId, string name, StateType type);

    void BuildTrigger(string id, string description);

    void BuildAction(string ownerId, string description, ActionType type);

    void BuildTransition(string id, string sourceId, string destinationId, string? triggerId, string? guard);

    FiniteStateMachine GetResult();
}
