using FsmViewer.Core.Domain;
using FsmViewer.Core.Validators;
using FsmViewer.Infrastructure;
using FsmViewer.Presentation;

namespace FsmViewer.Tests;

public sealed class FsmViewerTests
{
    [Fact]
    public void ExampleLampIsParsedWithExpectedCountsAndTypes()
    {
        FiniteStateMachine fsm = LoadFixture("example_lamp.fsm");

        Assert.Equal(5, fsm.GetAllStates().Count);
        Assert.Single(fsm.GetAllStates().OfType<InitialState>());
        Assert.Single(fsm.GetAllStates().OfType<CompoundState>());
        Assert.Equal(2, fsm.GetAllStates().OfType<SimpleState>().Count());
        Assert.Single(fsm.GetAllStates().OfType<FinalState>());
        Assert.Equal(3, fsm.GetTriggers().Count);
        Assert.Equal(4, fsm.GetActions().Count);
        Assert.Equal(4, fsm.GetTransitions().Count);
    }

    [Fact]
    public void ExampleLampPlacesOffAndOnInsidePoweredCompoundState()
    {
        FiniteStateMachine fsm = LoadFixture("example_lamp.fsm");

        CompoundState powered = Assert.IsType<CompoundState>(fsm.GetState("powered"));

        Assert.Contains(powered.GetChildren(), state => state.Id == "off");
        Assert.Contains(powered.GetChildren(), state => state.Id == "on");
    }

    [Fact]
    public void ExampleLampTransitionCanHaveTriggerGuardAndEffect()
    {
        FiniteStateMachine fsm = LoadFixture("example_lamp.fsm");

        Transition transition = Assert.IsType<Transition>(fsm.GetTransition("t2"));

        Assert.Equal("push_switch", transition.Trigger?.Id);
        Assert.Equal("time off > 10s", transition.Guard);
        Assert.Equal("reset off timer", transition.Effect?.Description);
    }

    [Fact]
    public void CompoundStateCanContainSimpleState()
    {
        var compound = new CompoundState("parent", "Parent");
        var child = new SimpleState("child", "Child");

        compound.Add(child);

        Assert.Same(child, Assert.Single(compound.GetChildren()));
        Assert.Same(compound, child.Parent);
    }

    [Fact]
    public void CompoundStateCanContainAnotherCompoundState()
    {
        var parent = new CompoundState("parent", "Parent");
        var child = new CompoundState("child", "Child");

        parent.Add(child);

        Assert.IsType<CompoundState>(Assert.Single(parent.GetChildren()));
        Assert.Same(parent, child.Parent);
    }

    [Fact]
    public void FactoryMethodCreatesSimpleState()
    {
        var creator = new SimpleStateCreator();

        StateComponent state = creator.Create("state", "State");

        Assert.IsType<SimpleState>(state);
    }

    [Fact]
    public void FactoryMethodCreatesCompoundState()
    {
        var creator = new CompoundStateCreator();

        StateComponent state = creator.Create("state", "State");

        Assert.IsType<CompoundState>(state);
    }

    [Fact]
    public void FsmBuilderUsesConfiguredStateCreatorAtRuntime()
    {
        var creator = new CountingStateCreator();
        var builder = new FsmBuilder(new Dictionary<StateType, Core.Interfaces.StateCreator>
        {
            [StateType.INITIAL] = creator,
            [StateType.SIMPLE] = creator,
            [StateType.COMPOUND] = creator,
            [StateType.FINAL] = creator
        });

        builder.Reset();
        builder.BuildState("state", "_", "State", StateType.SIMPLE);

        Assert.Equal(1, creator.CreateStateCallCount);
        Assert.IsType<SimpleState>(Assert.Single(builder.GetResult().GetStates()));
    }

    [Theory]
    [InlineData("example_lamp.fsm")]
    [InlineData("example_user_account.fsm")]
    [InlineData("invalid_compound.fsm")]
    [InlineData("invalid_deterministic1.fsm")]
    [InlineData("invalid_deterministic2.fsm")]
    [InlineData("invalid_deterministic3.fsm")]
    [InlineData("invalid_final.fsm")]
    [InlineData("invalid_initial.fsm")]
    [InlineData("invalid_unreachable.fsm")]
    [InlineData("valid_compound.fsm")]
    [InlineData("valid_deterministic.fsm")]
    public void AllProvidedFixturesCanBeParsedAndBuilt(string fixtureName)
    {
        FiniteStateMachine fsm = LoadFixture(fixtureName);

        Assert.NotEmpty(fsm.GetAllStates());
        Assert.NotEmpty(fsm.GetTransitions());
    }

    [Fact]
    public void LoaderReportsUnknownParentWithLineContext()
    {
        string path = CreateTempFsmFile(
            "STATE child missing_parent \"Child\" : SIMPLE;");

        try
        {
            FsmBuildException exception = Assert.Throws<FsmBuildException>(() => new FsmLoader().Load(path));

            Assert.Contains("Line 1", exception.Message);
            Assert.Contains("unknown parent state 'missing_parent'", exception.Message);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoaderReportsUnknownActionOwnerWithLineContext()
    {
        string path = CreateTempFsmFile(
            "STATE state1 _ \"State 1\" : SIMPLE;",
            "ACTION missing_state \"entry\" : ENTRY_ACTION;");

        try
        {
            FsmBuildException exception = Assert.Throws<FsmBuildException>(() => new FsmLoader().Load(path));

            Assert.Contains("Line 2", exception.Message);
            Assert.Contains("unknown state owner 'missing_state'", exception.Message);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoaderReportsUnknownTransitionTriggerWithLineContext()
    {
        string path = CreateTempFsmFile(
            "STATE state1 _ \"State 1\" : SIMPLE;",
            "STATE state2 _ \"State 2\" : SIMPLE;",
            "TRANSITION t1 state1 -> state2 missing_trigger \"\";");

        try
        {
            FsmBuildException exception = Assert.Throws<FsmBuildException>(() => new FsmLoader().Load(path));

            Assert.Contains("Line 3", exception.Message);
            Assert.Contains("unknown trigger 'missing_trigger'", exception.Message);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoaderReportsTransitionActionWithoutMatchingTransition()
    {
        string path = CreateTempFsmFile(
            "STATE state1 _ \"State 1\" : SIMPLE;",
            "ACTION t_missing \"effect\" : TRANSITION_ACTION;");

        try
        {
            FsmBuildException exception = Assert.Throws<FsmBuildException>(() => new FsmLoader().Load(path));

            Assert.Contains("unknown transition owner", exception.Message);
            Assert.Contains("t_missing", exception.Message);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Theory]
    [InlineData("invalid_deterministic1.fsm", typeof(DeterminismValidator))]
    [InlineData("invalid_deterministic2.fsm", typeof(DeterminismValidator))]
    [InlineData("invalid_deterministic3.fsm", typeof(DeterminismValidator))]
    [InlineData("invalid_initial.fsm", typeof(InitialFinalValidator))]
    [InlineData("invalid_final.fsm", typeof(InitialFinalValidator))]
    [InlineData("invalid_compound.fsm", typeof(CompoundTransitionValidator))]
    public void InvalidFixturesAreRejectedByExpectedValidator(string fixtureName, Type validatorType)
    {
        FiniteStateMachine fsm = LoadFixture(fixtureName);
        FsmValidator validator = (FsmValidator)Activator.CreateInstance(validatorType)!;

        validator.Validate(fsm);

        Assert.NotEmpty(validator.GetErrors());
    }

    [Theory]
    [InlineData("example_lamp.fsm")]
    [InlineData("example_user_account.fsm")]
    [InlineData("valid_compound.fsm")]
    [InlineData("valid_deterministic.fsm")]
    public void ValidFixturesPassAllImplementedValidators(string fixtureName)
    {
        FiniteStateMachine fsm = LoadFixture(fixtureName);

        IReadOnlyList<ValidationError> errors = Validate(fsm);

        Assert.Empty(errors);
    }

    [Fact]
    public void InvalidUnreachableIsNotRejectedBecauseReachabilityValidatorIsOutOfScope()
    {
        FiniteStateMachine fsm = LoadFixture("invalid_unreachable.fsm");

        IReadOnlyList<ValidationError> errors = Validate(fsm);

        Assert.Empty(errors);
    }

    [Fact]
    public void ExampleUserAccountSupportsMoreThanOneLevelOfCompoundNesting()
    {
        FiniteStateMachine fsm = LoadFixture("example_user_account.fsm");
        StateComponent unverified = Assert.IsType<SimpleState>(fsm.GetState("unverified"));

        Assert.Equal("inactive", unverified.Parent?.Id);
        Assert.Equal("created", unverified.Parent?.Parent?.Id);
    }

    [Fact]
    public void SelfTransitionCanExist()
    {
        FiniteStateMachine fsm = LoadFixture("example_user_account.fsm");

        Transition transition = Assert.IsType<Transition>(fsm.GetTransition("t3"));

        Assert.Same(transition.Source, transition.Destination);
    }

    [Fact]
    public void AutomaticTransitionHasNoTrigger()
    {
        FiniteStateMachine fsm = LoadFixture("invalid_compound.fsm");

        Transition transition = Assert.IsType<Transition>(fsm.GetTransition("t1"));

        Assert.True(transition.IsAutomatic());
        Assert.Null(transition.Trigger);
    }

    [Fact]
    public void DeterminismValidatorRejectsAutomaticTransitionsWithSameGuard()
    {
        var state1 = new SimpleState("state1", "State 1");
        var state2 = new SimpleState("state2", "State 2");
        var state3 = new SimpleState("state3", "State 3");
        var fsm = new FiniteStateMachine();
        fsm.AddState(state1);
        fsm.AddState(state2);
        fsm.AddState(state3);
        fsm.AddTransition(new Transition("t1", state1, state2, null, "x > 3", null));
        fsm.AddTransition(new Transition("t2", state1, state3, null, "x > 3", null));
        var validator = new DeterminismValidator();

        validator.Validate(fsm);

        Assert.Contains(validator.GetErrors(), error => error.Message.Contains("Multiple automatic transitions"));
    }

    [Fact]
    public void TextRenderVisitorCanRenderSingleState()
    {
        FiniteStateMachine fsm = LoadFixture("example_lamp.fsm");
        var presenter = new TextFsmPresenter();

        string text = presenter.Present(fsm.GetState("on")!);

        Assert.Contains("Simple state on", text);
        Assert.Contains("On Entry: Turn lamp on", text);
        Assert.Contains("On Exit: Turn lamp off", text);
        Assert.Contains("Incoming transitions:", text);
        Assert.Contains("Outgoing transitions:", text);
        Assert.Contains("Transition t2", text);
        Assert.Contains("Transition t3", text);
    }

    [Fact]
    public void TextRenderVisitorCanRenderSingleTransition()
    {
        FiniteStateMachine fsm = LoadFixture("example_lamp.fsm");
        var presenter = new TextFsmPresenter();

        string text = presenter.Present(fsm.GetTransition("t2")!);

        Assert.Contains("Transition t2", text);
        Assert.Contains("Push switch", text);
        Assert.Contains("time off > 10s", text);
        Assert.Contains("reset off timer", text);
    }

    [Fact]
    public void TextFsmPresenterCanRenderCompleteFsm()
    {
        FiniteStateMachine fsm = LoadFixture("example_user_account.fsm");
        var presenter = new TextFsmPresenter();

        string text = presenter.Present(fsm);

        Assert.Contains("Finite State Machine", text);
        Assert.Contains("Compound state created", text);
        Assert.Contains("Compound state inactive", text);
        Assert.Contains("Transitions:", text);
        Assert.Contains("Transition t12", text);
    }

    private static FiniteStateMachine LoadFixture(string fixtureName)
    {
        return new FsmLoader().Load(Path.Combine(GetFixtureDirectory(), fixtureName));
    }

    private static IReadOnlyList<ValidationError> Validate(FiniteStateMachine fsm)
    {
        var service = new FsmValidationService([
            new DeterminismValidator(),
            new InitialFinalValidator(),
            new CompoundTransitionValidator()
        ]);

        return service.Validate(fsm);
    }

    private static string GetFixtureDirectory()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, "Test FSMs");

            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find Test FSMs fixture directory.");
    }

    private static string CreateTempFsmFile(params string[] lines)
    {
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.fsm");
        File.WriteAllLines(path, lines);
        return path;
    }

    private sealed class CountingStateCreator : Core.Interfaces.StateCreator
    {
        public int CreateStateCallCount { get; private set; }

        protected override StateComponent CreateState(string id, string name)
        {
            CreateStateCallCount++;
            return new SimpleState(id, name);
        }
    }
}
