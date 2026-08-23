using FsmViewer.Core.Domain;
using FsmViewer.Core.Interfaces;

namespace FsmViewer.Infrastructure;

public sealed class CompoundStateCreator : StateCreator
{
    protected override StateComponent CreateState(string id, string name)
    {
        return new CompoundState(id, name);
    }
}
