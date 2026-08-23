using FsmViewer.Core.Domain;

namespace FsmViewer.Core.Validators;

public abstract class FsmValidator
{
    private readonly List<ValidationError> errors = [];

    public void Validate(FiniteStateMachine fsm)
    {
        errors.Clear();
        ValidateStates(fsm);
        ValidateTransitions(fsm);
    }

    public IReadOnlyList<ValidationError> GetErrors()
    {
        return errors;
    }

    protected virtual void ValidateStates(FiniteStateMachine fsm)
    {
    }

    protected virtual void ValidateTransitions(FiniteStateMachine fsm)
    {
    }

    protected void AddError(string message, string? elementId = null)
    {
        errors.Add(new ValidationError(message, elementId));
    }
}
