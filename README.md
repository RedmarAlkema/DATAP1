# FSM Viewer

C# consoleapplicatie voor de Design Patterns 1 FSM-eindopdracht.

## Build en tests

```powershell
dotnet build FsmViewer.sln
dotnet test FsmViewer.sln
```

## Programma draaien

```powershell
dotnet run --project src\FsmViewer\FsmViewer.csproj -- "Test FSMs\example_lamp.fsm"
```

Voorbeelden van ongeldige FSM's:

```powershell
dotnet run --project src\FsmViewer\FsmViewer.csproj -- "Test FSMs\invalid_deterministic1.fsm"
dotnet run --project src\FsmViewer\FsmViewer.csproj -- "Test FSMs\invalid_initial.fsm"
dotnet run --project src\FsmViewer\FsmViewer.csproj -- "Test FSMs\invalid_final.fsm"
dotnet run --project src\FsmViewer\FsmViewer.csproj -- "Test FSMs\invalid_compound.fsm"
```

## Design patterns

- Composite: `StateComponent` is de component, `SimpleState`, `InitialState` en `FinalState` zijn leaves, `CompoundState` is de composite.
- Visitor: `IFsmVisitor` en `IFsmElement` scheiden rendering van het domeinmodel; `TextRenderVisitor` maakt de tekstuele output.
- Builder: `FsmParser` maakt tokens, `FsmDirector` stuurt `IFsmBuilder`, `FsmBuilder` bouwt de `FiniteStateMachine`.
- Factory Method: `StateCreator` maakt states via concrete creators, gekozen via een dictionary in `FsmBuilder`.
- Template Method: `FsmValidator.Validate()` bepaalt de validatievolgorde; concrete validators vullen alleen de relevante stappen in.
- Strategy: `FsmValidationService` voert een verwisselbare set validators uit.

## Scope

Alle must-have requirements zijn geimplementeerd. De nice-to-have grafische interface en simulatie zijn bewust niet toegevoegd.
