using FsmViewer.Core.Domain;

namespace FsmViewer.Core.Interfaces;

public abstract class StateCreator
{
    public StateComponent Create(string id, string name)
    {
        return CreateState(id, name);
    }

    protected abstract StateComponent CreateState(string id, string name);
}
