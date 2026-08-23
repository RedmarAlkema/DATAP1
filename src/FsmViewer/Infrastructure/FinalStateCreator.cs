using FsmViewer.Core.Domain;
using FsmViewer.Core.Interfaces;

namespace FsmViewer.Infrastructure;

public sealed class FinalStateCreator : StateCreator
{
    protected override StateComponent CreateState(string id, string name)
    {
        return new FinalState(id, name);
    }
}
