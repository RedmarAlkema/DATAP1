using FsmViewer.Core.Domain;

namespace FsmViewer.Core.Validators;

public sealed class FsmValidationService
{
    private readonly List<FsmValidator> validators;

    public FsmValidationService(IEnumerable<FsmValidator> validators)
    {
        this.validators = validators.ToList();
    }

    public IReadOnlyList<ValidationError> Validate(FiniteStateMachine fsm)
    {
        var errors = new List<ValidationError>();

        foreach (FsmValidator validator in validators)
        {
            validator.Validate(fsm);
            errors.AddRange(validator.GetErrors());
        }

        return errors;
    }
}
