# Architectuur

```text
Core
├── Interfaces
├── Domain
└── Validators

Infrastructure

Presentation
```

Afhankelijkheden horen hoofdzakelijk deze richting op te lopen:

```text
Presentation   ──> Core
Infrastructure ──> Core
```

`Core` hoort niet afhankelijk te zijn van concrete klassen uit `Presentation` of `Infrastructure`.

---

# 1. Core / Interfaces

## IFsmBuilder

* **Map:** `Core/Interfaces`
* **Naam:** `IFsmBuilder`
* **Type:** interface
* **Parameters:** geen
* **Methodes:**

  * `+ Reset()`
  * `+ BuildState(id, parentId, name, type)`
  * `+ BuildTrigger(id, description)`
  * `+ BuildAction(ownerId, description, type)`
  * `+ BuildTransition(id, sourceId, destinationId, triggerId, guard)`
* **Connecties:**

  * `FsmBuilder --▷ IFsmBuilder` — interface-implementatie
  * `FsmDirector -> IFsmBuilder` — associatie; Director bewaart een Builder
* **Patroon:** **Builder**
* **Rol:** Builder interface

---

## IFsmElement

* **Map:** `Core/Interfaces`
* **Naam:** `IFsmElement`
* **Type:** interface
* **Parameters:** geen
* **Methodes:**

  * `+ Accept(visitor: IFsmVisitor)`
* **Connecties:**

  * `StateComponent --▷ IFsmElement` — implementatie
  * `Transition --▷ IFsmElement` — implementatie
  * gebruikt `IFsmVisitor` als parameter
* **Patroon:** **Visitor**
* **Rol:** Element interface

---

## IFsmVisitor

* **Map:** `Core/Interfaces`
* **Naam:** `IFsmVisitor`
* **Type:** interface
* **Parameters:** geen
* **Methodes:**

  * `+ Visit(state: SimpleState)`
  * `+ Visit(state: CompoundState)`
  * `+ Visit(state: InitialState)`
  * `+ Visit(state: FinalState)`
  * `+ Visit(transition: Transition)`
* **Connecties:**

  * `TextRenderVisitor --▷ IFsmVisitor` — implementatie
  * gebruikt de concrete FSM-elementen als parameters
* **Patroon:** **Visitor**
* **Rol:** Visitor interface

---

## IFsmPresenter

* **Map:** `Core/Interfaces`
* **Naam:** `IFsmPresenter`
* **Type:** interface
* **Parameters:** geen
* **Methodes:**

  * `+ Present(fsm: FiniteStateMachine)`
  * `+ Present(state: StateComponent)`
  * `+ Present(transition: Transition)`
* **Connecties:**

  * `TextFsmPresenter --▷ IFsmPresenter` — implementatie
* **Patroon:** geen GoF-patroon
* **Doel:** Presentation en model los van elkaar houden.

---

## StateCreator

* **Map:** `Core/Interfaces`
* **Naam:** `StateCreator`
* **Type:** abstracte klasse
* **Parameters:** geen
* **Methodes:**

  * `+ Create(id: string, name: string): StateComponent`
  * `# CreateState(id: string, name: string): StateComponent`
* **Connecties:**

  * `SimpleStateCreator ─▷ StateCreator` — overerving
  * `InitialStateCreator ─▷ StateCreator` — overerving
  * `FinalStateCreator ─▷ StateCreator` — overerving
  * `CompoundStateCreator ─▷ StateCreator` — overerving
  * dependency naar `StateComponent`
  * `FsmBuilder -> StateCreator` — gebruikt creators
* **Patroon:** **Factory Method**
* **Rol:** Creator
* `CreateState()` is de protected Factory Method die door subclasses wordt overridden.

---

# 2. Core / Domain

## FiniteStateMachine

* **Map:** `Core/Domain`
* **Naam:** `FiniteStateMachine`
* **Type:** klasse
* **Parameters/velden:**

  * `- states: List<StateComponent>`
  * `- transitions: List<Transition>`
  * `- triggers: List<FsmTrigger>`
  * `- actions: List<FsmAction>`
* **Methodes:**

  * `+ AddState(state: StateComponent)`
  * `+ AddTransition(transition: Transition)`
  * `+ AddTrigger(trigger: FsmTrigger)`
  * `+ AddAction(action: FsmAction)`
  * `+ GetTransitions(): IReadOnlyList<Transition>`
  * `+ GetStates(): IReadOnlyList<StateComponent>`
* **Connecties:**

  * associatie naar `StateComponent`
  * bevat meerdere `Transition`
  * associatie `0..*` naar `FsmTrigger`
  * associatie `0..*` naar `FsmAction`
  * wordt gebouwd door `FsmBuilder`
* **Patroon:** **Composite**
* **Rol:** Client/Product-container

---

## StateComponent

* **Map:** `Core/Domain`
* **Naam:** `StateComponent`
* **Type:** abstracte klasse
* **Parameters/properties:**

  * `+ Id: string { get; }`
  * `+ Name: string { get; }`
* **Methodes:**

  * `+ Accept(visitor: IFsmVisitor)` — abstract/overridebaar
  * `+ AddOutgoingTransition(transition: Transition)`
  * `+ AddIncomingTransition(transition: Transition)`
* **Connecties:**

  * implementeert `IFsmElement`
  * parent van `SimpleState`
  * parent van `InitialState`
  * parent van `FinalState`
  * parent van `CompoundState`
  * wordt gebruikt als producttype door `StateCreator`
* **Patronen:**

  * **Composite** — Component
  * **Factory Method** — Product

De concrete states erven `Id`, `Name` en transition-functionaliteit.

---

## SimpleState

* **Map:** `Core/Domain`
* **Naam:** `SimpleState`
* **Type:** klasse
* **Parameters:** overgenomen van `StateComponent`
* **Geërfde methodes:**

  * `AddOutgoingTransition(...)`
  * `AddIncomingTransition(...)`
* **Te implementeren/overriden methode:**

  * `+ Accept(visitor: IFsmVisitor)` → `visitor.Visit(this)`
* **Connecties:**

  * erft van `StateComponent`
  * associatie `0..*` naar `FsmAction`
  * wordt aangemaakt door `SimpleStateCreator`
* **Patronen:**

  * **Composite** — Leaf
  * **Visitor** — Concrete Element
  * **Factory Method** — Concrete Product

---

## InitialState

* **Map:** `Core/Domain`
* **Naam:** `InitialState`
* **Type:** klasse
* **Parameters:** overgenomen van `StateComponent`
* **Geërfde methodes:**

  * `AddOutgoingTransition(...)`
  * `AddIncomingTransition(...)`
* **Te implementeren/overriden methode:**

  * `+ Accept(visitor: IFsmVisitor)` → `visitor.Visit(this)`
* **Connecties:**

  * erft van `StateComponent`
  * wordt aangemaakt door `InitialStateCreator`
* **Patronen:**

  * **Composite** — Leaf
  * **Visitor** — Concrete Element
  * **Factory Method** — Concrete Product

---

## FinalState

* **Map:** `Core/Domain`
* **Naam:** `FinalState`
* **Type:** klasse
* **Parameters:** overgenomen van `StateComponent`
* **Geërfde methodes:**

  * `AddOutgoingTransition(...)`
  * `AddIncomingTransition(...)`
* **Te implementeren/overriden methode:**

  * `+ Accept(visitor: IFsmVisitor)` → `visitor.Visit(this)`
* **Connecties:**

  * erft van `StateComponent`
  * wordt aangemaakt door `FinalStateCreator`
* **Patronen:**

  * **Composite** — Leaf
  * **Visitor** — Concrete Element
  * **Factory Method** — Concrete Product

---

## CompoundState

* **Map:** `Core/Domain`
* **Naam:** `CompoundState`
* **Type:** klasse
* **Parameters/velden:**

  * `- children: List<StateComponent>`
* **Methodes:**

  * `+ Add(state: StateComponent)`
  * `+ Remove(state: StateComponent)`
  * `+ GetChildren(): IReadOnlyList<StateComponent>`
  * `+ GetParent()`
  * `+ Accept(visitor: IFsmVisitor)` — geërfd contract, moet worden overridden
* **Connecties:**

  * erft van `StateComponent`
  * aggregatie `0..*` naar `StateComponent`
  * wordt aangemaakt door `CompoundStateCreator`
* **Patronen:**

  * **Composite** — Composite
  * **Visitor** — Concrete Element
  * **Factory Method** — Concrete Product

Omdat `children` uit `StateComponent` bestaat, kan een `CompoundState` opnieuw een `CompoundState` bevatten.

---

## Transition

* **Map:** `Core/Domain`
* **Naam:** `Transition`
* **Type:** klasse
* **Parameters/properties:**

  * `+ Id: string`
  * `+ Source: StateComponent { get; }`
  * `+ Destination: StateComponent { get; }`
  * `+ Trigger: FsmTrigger? { get; }`
  * `+ Guard: string? { get; }`
  * `+ Effect: FsmAction? { get; }`
* **Methodes:**

  * `+ Accept(visitor: IFsmVisitor)`
  * `+ IsAutomatic()`
  * `+ HasGuard()`
* **Connecties:**

  * implementeert `IFsmElement`
  * associatie `0..1` naar `FsmTrigger`
  * associatie `0..1` naar `FsmAction`
  * verwijst naar een source en destination `StateComponent`
* **Patroon:** **Visitor**
* **Rol:** Concrete Element

Een self-transition is gewoon een `Transition` waarbij:

```text
Source == Destination
```

---

## FsmTrigger

* **Map:** `Core/Domain`
* **Naam:** `FsmTrigger`
* **Type:** klasse
* **Parameters/properties:**

  * `+ Id: string`
  * `+ Description: string`
* **Methodes:** geen specifieke nodig
* **Connecties:**

  * `Transition -> FsmTrigger` met multipliciteit `0..1`
  * `FiniteStateMachine -> FsmTrigger` met `0..*`
* **Patroon:** geen
* **Doel:** trigger van een transition representeren.

---

## FsmAction

* **Map:** `Core/Domain`
* **Naam:** `FsmAction`
* **Type:** klasse
* **Parameters/properties:**

  * `+ Description: string`
  * `+ Type: ActionType`
* **Methodes:** geen specifieke nodig
* **Connecties:**

  * `SimpleState -> FsmAction` met `0..*`
  * `Transition -> FsmAction` met `0..1` als effect
  * `FiniteStateMachine -> FsmAction` met `0..*`
  * gebruikt `ActionType`
* **Patroon:** geen

---

## ActionType

* **Map:** `Core/Domain`
* **Naam:** `ActionType`
* **Type:** enum
* **Waarden:**

  * `ENTRY_ACTION`
  * `DO_ACTION`
  * `TRANSITION_ACTION`
  * `EXIT_ACTION`
* **Connecties:**

  * gebruikt door `FsmAction.Type`
* **Patroon:** geen

---

## StateType

* **Map:** `Core/Domain`
* **Naam:** `StateType`
* **Type:** enum
* **Waarden:**

  * `INITIAL`
  * `SIMPLE`
  * `COMPOUND`
  * `FINAL`
* **Connecties:**

  * gebruikt door `FsmBuilder.BuildState(...)`
  * bepaalt welke `StateCreator` gebruikt wordt
* **Patroon:** ondersteunend aan **Factory Method**

---

# 3. Core / Validators

## FsmValidator

* **Map:** `Core/Validators`
* **Naam:** `FsmValidator`
* **Type:** abstracte klasse
* **Parameters/velden:**

  * `- errors: List<ValidationError>`
* **Methodes:**

  * `+ Validate(fsm: FiniteStateMachine)` — Template Method
  * `# ValidateStates(fsm: FiniteStateMachine)`
  * `# ValidateTransitions(fsm: FiniteStateMachine)`
  * `# AddError(message: string)`
  * `+ GetErrors()`
* **Connecties:**

  * associatie `0..*` naar `ValidationError`
  * parent van de drie concrete validators
  * gebruikt door `FsmValidationService`
* **Patroon:** **Template Method**
* **Rol:** Abstract Class

`Validate()` bepaalt de vaste volgorde. Subclasses overriden alleen de benodigde stappen.

---

## DeterminismValidator

* **Map:** `Core/Validators`
* **Naam:** `DeterminismValidator`
* **Type:** klasse
* **Parameters:** geen
* **Geërfde methodes:**

  * `Validate(...)`
  * `GetErrors()`
  * `AddError(...)`
  * `ValidateTransitions(...)`
* **Override:**

  * `# ValidateStates(fsm: FiniteStateMachine)`
* **Connecties:**

  * erft van `FsmValidator`
* **Patroon:** **Template Method**
* **Rol:** Concrete Class

Controleert niet-deterministische uitgaande transitions.

---

## InitialFinalValidator

* **Map:** `Core/Validators`
* **Naam:** `InitialFinalValidator`
* **Type:** klasse
* **Parameters:** geen
* **Geërfde methodes:**

  * `Validate(...)`
  * `GetErrors()`
  * `AddError(...)`
  * `ValidateTransitions(...)`
* **Override:**

  * `# ValidateStates(fsm: FiniteStateMachine)`
* **Connecties:**

  * erft van `FsmValidator`
* **Patroon:** **Template Method**
* **Rol:** Concrete Class

Controleert initial/final-regels.

---

## CompoundTransitionValidator

* **Map:** `Core/Validators`
* **Naam:** `CompoundTransitionValidator`
* **Type:** klasse
* **Parameters:** geen
* **Geërfde methodes:**

  * `Validate(...)`
  * `GetErrors()`
  * `AddError(...)`
  * `ValidateStates(...)`
* **Override:**

  * `# ValidateTransitions(fsm: FiniteStateMachine)`
* **Connecties:**

  * erft van `FsmValidator`
* **Patroon:** **Template Method**
* **Rol:** Concrete Class

Controleert dat een transition niet eindigt op een `CompoundState`.

---

## FsmValidationService

* **Map:** `Core/Validators`
* **Naam:** `FsmValidationService`
* **Type:** klasse
* **Parameters/velden:**

  * `- validators: List<FsmValidator>`
* **Methodes:**

  * `+ FsmValidationService(validators: List<FsmValidator>)`
  * `+ Validate(fsm: FiniteStateMachine)`
* **Connecties:**

  * associatie naar `0..* FsmValidator`
* **Patroon:** geen directe patroonrol
* **Doel:** alle validators centraal uitvoeren.

---

## ValidationError

* **Map:** `Core/Validators`
* **Naam:** `ValidationError`
* **Type:** klasse
* **Huidige diagram-inhoud:** bevat momenteel ten onrechte opnieuw een `List<ValidationError>`
* **Aanbevolen parameters:**

  * `+ Message: string`
  * eventueel `+ ElementId: string?`
* **Connecties:**

  * `FsmValidator -> ValidationError` met `0..*`
* **Patroon:** geen

---

# 4. Infrastructure

## FsmBuilder

* **Map:** `Infrastructure`
* **Naam:** `FsmBuilder`
* **Type:** klasse
* **Parameters/velden:**

  * `- result: FiniteStateMachine`
* **Methodes zichtbaar:**

  * `+ GetResult()`
  * `+ BuildState(id, parentId, name, type: StateType)`
* **Methodes die door IFsmBuilder verplicht ook geïmplementeerd moeten worden:**

  * `+ Reset()`
  * `+ BuildTrigger(id, description)`
  * `+ BuildAction(ownerId, description, type)`
  * `+ BuildTransition(id, sourceId, destinationId, triggerId, guard)`
* **Connecties:**

  * implementeert `IFsmBuilder`
  * associatie naar `FiniteStateMachine`
  * associatie naar `StateCreator`
  * dependency naar `StateType`
  * wordt gebruikt door `FsmLoader`
* **Patronen:**

  * **Builder** — Concrete Builder
  * gebruikt de **Factory Method** creators

---

## FsmDirector

* **Map:** `Infrastructure`
* **Naam:** `FsmDirector`
* **Type:** klasse
* **Parameters/velden:**

  * `- builder: IFsmBuilder`
* **Methodes:**

  * `+ FsmDirector(builder: IFsmBuilder)`
  * `+ Construct(tokens: List<FsmToken>)`
* **Connecties:**

  * associatie naar `IFsmBuilder`
  * dependency naar `FsmToken`
  * wordt gebruikt door `FsmLoader`
* **Patroon:** **Builder**
* **Rol:** Director

---

## FsmLoader

* **Map:** `Infrastructure`
* **Naam:** `FsmLoader`
* **Type:** klasse
* **Parameters:** geen
* **Methodes:**

  * `+ Load(filePath: string): FiniteStateMachine`
* **Connecties:**

  * dependency naar `FsmParser`
  * dependency naar `FsmBuilder`
  * associatie naar `FsmDirector`
* **Patroon:** **Builder**
* **Rol:** Client

Flow:

```text
FsmLoader
    ↓
FsmParser
    ↓
FsmToken[]
    ↓
FsmDirector
    ↓
FsmBuilder
    ↓
FiniteStateMachine
```

---

## FsmParser

* **Map:** `Infrastructure`
* **Naam:** `FsmParser`
* **Type:** klasse
* **Parameters:** geen
* **Methodes:**

  * `+ Parse(filePath: string)`
* **Aanbevolen returntype:**

  * `List<FsmToken>`
* **Connecties:**

  * dependency naar `FsmToken`
  * gebruikt door `FsmLoader`
* **Patroon:** geen
* **Doel:** tekstregels uit `.fsm` omzetten naar tokens.

---

## FsmToken

* **Map:** `Infrastructure`
* **Naam:** `FsmToken`
* **Type:** klasse
* **Parameters/properties:**

  * `+ Type: LineType { get; }`
  * `+ Tokens: List<string> { get; }`
* **Methodes:** geen specifieke nodig
* **Connecties:**

  * gebruikt `LineType`
  * aangemaakt door `FsmParser`
  * gebruikt door `FsmDirector`
* **Patroon:** geen

---

## LineType

* **Map:** `Infrastructure`
* **Naam:** `LineType`
* **Type:** enum
* **Waarden:**

  * `STATE`
  * `TRIGGER`
  * `ACTION`
  * `TRANSITION`
* **Connecties:**

  * gebruikt door `FsmToken.Type`
* **Patroon:** geen

---

## SimpleStateCreator

* **Map:** `Infrastructure`
* **Naam:** `SimpleStateCreator`
* **Type:** klasse
* **Geërfde methode:**

  * `Create(id, name)`
* **Override:**

  * `# CreateState(id, name): StateComponent`
* **Connecties:**

  * erft van `StateCreator`
  * dependency naar `SimpleState`
* **Patroon:** **Factory Method**
* **Rol:** Concrete Creator
* Maakt `new SimpleState(...)`.

---

## InitialStateCreator

* **Map:** `Infrastructure`
* **Naam:** `InitialStateCreator`
* **Type:** klasse
* **Geërfde methode:**

  * `Create(id, name)`
* **Override:**

  * `# CreateState(id, name): StateComponent`
* **Connecties:**

  * erft van `StateCreator`
  * dependency naar `InitialState`
* **Patroon:** **Factory Method**
* **Rol:** Concrete Creator

---

## FinalStateCreator

* **Map:** `Infrastructure`
* **Naam:** `FinalStateCreator`
* **Type:** klasse
* **Geërfde methode:**

  * `Create(id, name)`
* **Override:**

  * `# CreateState(id, name): StateComponent`
* **Connecties:**

  * erft van `StateCreator`
  * dependency naar `FinalState`
* **Patroon:** **Factory Method**
* **Rol:** Concrete Creator

---

## CompoundStateCreator

* **Map:** `Infrastructure`
* **Naam:** `CompoundStateCreator`
* **Type:** klasse
* **Geërfde methode:**

  * `Create(id, name)`
* **Override:**

  * `# CreateState(id, name): StateComponent`
* **Connecties:**

  * erft van `StateCreator`
  * dependency naar `CompoundState`
* **Patroon:** **Factory Method**
* **Rol:** Concrete Creator

---

# 5. Presentation

## TextRenderVisitor

* **Map:** `Presentation`
* **Naam:** `TextRenderVisitor`
* **Type:** klasse
* **Parameters/velden:**

  * `- output: StringBuilder`
* **Eigen methode:**

  * `+ GetOutput()`
* **Methodes uit IFsmVisitor die ook geïmplementeerd moeten worden:**

  * `+ Visit(state: SimpleState)`
  * `+ Visit(state: CompoundState)`
  * `+ Visit(state: InitialState)`
  * `+ Visit(state: FinalState)`
  * `+ Visit(transition: Transition)`
* **Connecties:**

  * implementeert `IFsmVisitor`
  * wordt gebruikt door `TextFsmPresenter`
* **Patroon:** **Visitor**
* **Rol:** Concrete Visitor

---

## TextFsmPresenter

* **Map:** `Presentation`
* **Naam:** `TextFsmPresenter`
* **Type:** klasse
* **Parameters:** geen
* **Methodes:**

  * `+ Present(fsm: FiniteStateMachine)`
  * `+ Present(state: StateComponent)`
  * `+ Present(transition: Transition)`
* **Connecties:**

  * implementeert `IFsmPresenter`
  * dependency naar `TextRenderVisitor`
  * gebruikt `IFsmElement`
* **Patroon:** **Visitor**
* **Rol:** Client

---

# Overzicht design patterns

## Composite

```text
Client       = FiniteStateMachine
Component    = StateComponent
Leaf         = SimpleState
Leaf         = InitialState
Leaf         = FinalState
Composite    = CompoundState
```

`CompoundState` bevat `0..* StateComponent`, waardoor nesting mogelijk is.

## Visitor

```text
Visitor interface = IFsmVisitor
Element interface = IFsmElement

Concrete Elements =
- SimpleState
- InitialState
- FinalState
- CompoundState
- Transition

Concrete Visitor = TextRenderVisitor
Client            = TextFsmPresenter
```

## Builder

```text
Builder          = IFsmBuilder
Concrete Builder = FsmBuilder
Director         = FsmDirector
Product          = FiniteStateMachine
Client           = FsmLoader
```

`FsmParser`, `FsmToken` en `LineType` ondersteunen het inlezen, maar zijn geen officiële Builder-rollen.

## Factory Method met late binding

```text
Creator = StateCreator

Concrete Creators =
- SimpleStateCreator
- InitialStateCreator
- FinalStateCreator
- CompoundStateCreator

Product = StateComponent

Concrete Products =
- SimpleState
- InitialState
- FinalState
- CompoundState
```

De late binding vindt plaats doordat `StateCreator.Create()` de overriden `CreateState()` aanroept.

## Template Method

```text
Abstract Class = FsmValidator
Template Method = Validate()

Concrete Classes =
- DeterminismValidator
- InitialFinalValidator
- CompoundTransitionValidator
```

---

# Nog aanpassen voordat Codex dit implementeert

Dit zijn de punten die ik in het huidige diagram nog zou corrigeren.

### 1. `initialState` hernoemen

Nu:

```text
initialState
```

Maak:

```text
InitialState
```

Voor consistente C#-naamgeving.

### 2. `IFsmpresenter` hernoemen

Nu:

```text
IFsmpresenter
```

Maak:

```text
IFsmPresenter
```

Verwijder ook de eventuele standaardmethode `Operation1()` die nog in het `.mdj`-bestand staat.

### 3. `CompoundState` moet echt erven van `StateComponent`

Dit moet een **generalization/overerving** zijn:

```text
CompoundState ─────▷ StateComponent
```

Dus volle lijn met open driehoek.

In het huidige `.mdj` staat deze relatie deels als realization/interface-implementatie opgeslagen. Dat moet worden gecorrigeerd omdat `StateComponent` een abstracte klasse is.

### 4. Gebruik overal `FsmTrigger` en `FsmAction`

In `Transition`:

```text
Trigger: FsmTrigger?
Effect: FsmAction?
```

Niet:

```text
Trigger
Action
```

Ook in `FiniteStateMachine`:

```text
List<FsmTrigger>
List<FsmAction>

AddTrigger(trigger: FsmTrigger)
AddAction(action: FsmAction)
```

### 5. Typfout in `FsmTrigger`

Gebruik:

```text
Description
```

niet `Discription`.

### 6. `ValidationError` corrigeren

De klasse hoort niet zelf dit te bevatten:

```text
List<ValidationError>
```

Maak bijvoorbeeld:

```text
ValidationError
+ Message: string
+ ElementId: string?
```

### 7. Transition-lijsten in `StateComponent`

Sterk aanbevolen:

```text
- incomingTransitions: List<Transition>
- outgoingTransitions: List<Transition>

+ GetIncomingTransitions(): IReadOnlyList<Transition>
+ GetOutgoingTransitions(): IReadOnlyList<Transition>
```

Dit maakt rendering en validatie veel eenvoudiger.

### 8. Parent van een state

`GetParent()` staat nu alleen bij `CompoundState`. Logischer is dat iedere state eventueel een compound parent kan hebben.

Bijvoorbeeld op `StateComponent`:

```text
+ Parent: CompoundState?
```

of:

```text
+ GetParent(): CompoundState?
```

Dan kan iedere geneste `SimpleState` of `CompoundState` zijn parent kennen.

### 9. Lookups op `FiniteStateMachine`

Sterk aanbevolen voor de Builder:

```text
+ GetState(id: string): StateComponent?
+ GetTrigger(id: string): FsmTrigger?
```

Eventueel ook:

```text
+ GetAllStates(): IReadOnlyList<StateComponent>
```

waarbij recursively door compound states wordt gelopen.

### 10. Late binding expliciet maken in `FsmBuilder`

Alleen een associatie naar `StateCreator` is mogelijk wat vaag. Voor de rubric zou ik duidelijk maken hoe runtime de juiste creator wordt gekozen.

Bijvoorbeeld:

```text
- stateCreators: Dictionary<StateType, StateCreator>
```

Daarmee kan:

```text
creator = stateCreators[type]
state = creator.Create(id, name)
```

De uiteindelijke `CreateState()` wordt daarna via polymorfisme runtime gekozen.

### 11. Returntype parser vastleggen

Maak expliciet:

```text
FsmParser.Parse(filePath: string): List<FsmToken>
```

### 12. Returntype validaties vastleggen

Maak duidelijk hoe fouten bij de caller terechtkomen. Bijvoorbeeld:

```text
FsmValidationService.Validate(
    fsm: FiniteStateMachine
): IReadOnlyList<ValidationError>
```

Dan is het gebruik van de validatorlaag ondubbelzinnig.