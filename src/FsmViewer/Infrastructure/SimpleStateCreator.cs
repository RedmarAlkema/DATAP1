using FsmViewer.Core.Domain;
using FsmViewer.Core.Interfaces;

namespace FsmViewer.Infrastructure;

public sealed class SimpleStateCreator : StateCreator
{
    protected override StateComponent CreateState(string id, string name)
    {
        return new SimpleState(id, name);
    }
}
