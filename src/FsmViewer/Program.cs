using FsmViewer.Core.Domain;
using FsmViewer.Core.Validators;
using FsmViewer.Infrastructure;
using FsmViewer.Presentation;

string? filePath = args.Length > 0 ? args[0] : null;

while (string.IsNullOrWhiteSpace(filePath))
{
    Console.Write("FSM file path: ");
    filePath = Console.ReadLine();
}

if (!File.Exists(filePath))
{
    Console.Error.WriteLine($"File not found: {filePath}");
    return 1;
}

try
{
    var loader = new FsmLoader();
    FiniteStateMachine fsm = loader.Load(filePath);
    var validationService = new FsmValidationService([
        new DeterminismValidator(),
        new InitialFinalValidator(),
        new CompoundTransitionValidator()
    ]);
    IReadOnlyList<ValidationError> errors = validationService.Validate(fsm);

    if (errors.Count > 0)
    {
        Console.WriteLine("Validation errors:");

        foreach (ValidationError error in errors)
        {
            Console.WriteLine($"- {error}");
        }

        return 2;
    }

    var presenter = new TextFsmPresenter();
    Console.WriteLine(presenter.Present(fsm));

    RunMenu(fsm, presenter);
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}

static void RunMenu(FiniteStateMachine fsm, TextFsmPresenter presenter)
{
    while (true)
    {
        Console.WriteLine("1. Show whole FSM");
        Console.WriteLine("2. Show state by id");
        Console.WriteLine("3. Show transition by id");
        Console.WriteLine("4. Exit");
        Console.Write("Choice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.WriteLine(presenter.Present(fsm));
                break;
            case "2":
                Console.Write("State id: ");
                string? stateId = Console.ReadLine();
                StateComponent? state = stateId is null ? null : fsm.GetState(stateId);
                Console.WriteLine(state is null ? "State not found." : presenter.Present(state));
                break;
            case "3":
                Console.Write("Transition id: ");
                string? transitionId = Console.ReadLine();
                Transition? transition = transitionId is null ? null : fsm.GetTransition(transitionId);
                Console.WriteLine(transition is null ? "Transition not found." : presenter.Present(transition));
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Unknown choice.");
                break;
        }
    }
}
