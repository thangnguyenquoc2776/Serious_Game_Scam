# Serious Game Scam - C# Codebase Architecture Overview

## Project Summary

A Unity-based educational game focused on teaching users to recognize and avoid scam tactics. The project implements a modular architecture with clear separation of concerns, using a service-oriented approach with dependency injection through a central `AppContext`.

---

## Directory Structure & Purposes

```
Assets/Scripts/
├── App/                      # Core application bootstrapping & context
├── Auth/                     # Authentication services (Firebase integration)
├── Content/                  # ScriptableObject configurations & taxonomies
├── Feedback/                 # Feedback & analytics report generation
├── Interactables/            # Interactive game objects (teleports, state flags)
├── Narrative/                # Story/dialogue system integration (Yarn)
├── Phone/                    # Phone UI interaction listeners
├── Player/                   # Player character controller
├── Runtime/                  # Core runtime interfaces & interactable system
├── Save/                     # Save/load game state management
├── State/                    # Player state management & game flags
├── Trace/                    # Event tracing & analytics data models
│   └── Dto/                  # Data transfer objects for traces
└── UI/                       # UI flow management, screen transitions, UI listeners
```

---

## Namespace Hierarchy & Relationships

### Level 1: Core Application (`SeriousGame.App`)

**Purpose:** Bootstrap, initialization, and central context management

**Key Classes:**

- `GameBootstrap` - Singleton entry point; initializes `AppContext` at game start
- `AppContext` - Central service container; manages all major services
- `SceneService` - Scene loading and navigation
- `SessionService` - Session management (session ID, participant ID)
- `GameEventBus` - Static event dispatcher for UI and system events

**File Count:** 6 files
**Dependencies:** All other namespaces (acts as orchestrator)

```
App Package Structure:
┌─────────────────┐
│ GameBootstrap   │ ◄── Entry point
├─────────────────┤
│ AppContext      │ ◄── Service container
├─────────────────┤
│ ServiceA        │
│ ServiceB        │
│ ServiceC        │
└─────────────────┘
```

---

### Level 2: Core Services (Primary Business Logic)

#### **SeriousGame.State** - Player State Management

**Purpose:** Manage player in-game state (scores, flags, variables)

**Key Classes:**

- `PlayerStateService` - Main state manager
  - Manages `Dictionary<string, int>` for numeric states
  - Manages `Dictionary<string, bool>` for boolean flags
  - Clamps values, validates keys
  - Includes default key initialization
- `PlayerStateSnapshot` - Serializable state snapshot
  - `List<PlayerStateEntry>` - State entries with key-value pairs
  - `List<PlayerFlagEntry>` - Flag entries

- `PlayerStateDebugger` - MonoBehaviour for debugging state in editor

- `GameStateKeys` - Constants for all valid state keys

**File Count:** 4 files
**Dependencies:** None (independent service)
**Dependents:** `Trace`, `Feedback`, `Save`, `UI`

---

#### **SeriousGame.Trace** - Event Tracing & Analytics

**Purpose:** Log and track all player actions and game events for analysis

**Architecture Pattern:** Strategy Pattern with multiple storage implementations

**Key Classes:**

- `TraceService` - Main trace manager
  - Dual-store pattern: in-memory (`_store`) + persistent (`_persistentStore`)
  - `BuildTrace()` - Creates trace events with metadata
  - `GetSession()` - Retrieves traces for a session
  - `SendSessionData()` - Exports session traces

- `ITraceStore` - Storage abstraction interface
  - `Add(GameTrace)` - Add trace event
  - `GetBySession(string)` - Query traces
  - `ClearSession(string)` - Clear session traces

- **Storage Implementations:**
  - `InMemoryTraceStore` - Fast, volatile storage
  - `FileTraceStore` - JSON file persistence
  - `CloudTraceStore` - Cloud/API persistence

- **Data Models (Dto):**
  - `GameTrace` - Single trace event
    - `milestone_id`, `route_id`, `choice_id` - Navigation context
    - `trace_id` - Action identifier
    - `timestamp` - Event time
    - `score_state` - Player state at event time
  - `ChapterSessionDTO` - Session-level summary
  - `FinalScores` - Final chapter scores

**File Count:** 6 files + DTOs
**Dependencies:** `State` (reads player state), `App` (session context)
**Dependents:** `Feedback`, `Save`, `UI`

---

#### **SeriousGame.Save** - Persistence Layer

**Purpose:** Save and load player progress

**Key Classes:**

- `SaveService` - Main save/load manager
  - `BuildCurrent()` - Create SaveData from current state
  - `Save()` - Write SaveData to file
  - `Load()` - Read SaveData from file
  - Integrates session, player state, and narrative context

- `SaveData` - Serializable game state
  - `sessionId`, `participantId`
  - `currentEpisodeId`, `currentUnityScene`
  - `currentYarnNode`, `currentMilestoneId`
  - `playerState` - PlayerStateSnapshot
  - `flags` - Additional save flags

**File Count:** 2 files
**Dependencies:** `App` (SessionService), `State` (PlayerStateService), `Narrative`
**Dependents:** UI flow

---

#### **SeriousGame.Feedback** - Analytics & Reporting

**Purpose:** Generate feedback reports based on player actions and traces

**Key Classes:**

- `FeedbackService` - Report generator
  - `GenerateEndChapterReport()` - Analyze traces and state
  - Scores based on `trace_id` occurrences weighted by taxonomy
  - Uses `FeedbackReport` for output

- `FeedbackReport` - Report data model
  - Contains aggregated feedback metrics
  - Based on risk weights from taxonomy

**File Count:** 2 files
**Dependencies:** `Trace`, `State`, `Content` (taxonomy)
**Dependents:** UI reporting

---

#### **SeriousGame.Narrative** - Dialogue & Story System

**Purpose:** Integrate with Yarn Spinner dialogue engine

**Key Classes:**

- `NarrativeService` (MonoBehaviour) - Story manager
  - Integrates with `DialogueRunner` (Yarn Spinner)
  - Manages current node/milestone tracking
  - `StartNode(nodeName)` - Begin dialogue
  - Locks/unlocks player controls during dialogue
  - Bridges to `PlayerController` for input management

- `YarnCommandBridge` - Custom Yarn commands
  - Implements narrative-triggered game commands

- `YarnAutoStart` - Auto-starts dialogue on scene load

**File Count:** 3 files
**Dependencies:** `App` (AppContext), `Runtime` (PlayerController)
**Dependents:** `Save`, `UI`, `Runtime`
**External Dependencies:** Yarn Spinner (yarn-spinner package)

---

### Level 3: Content & Configuration (`SeriousGame.Content`)

**Purpose:** Centralized configuration and evidence mapping

**Key Classes:**

- `AppConfigSO` (ScriptableObject) - Global game configuration
  - Scene names (boot, menu, episodes)
  - Firebase credentials
  - Flow settings (auto-start options)
  - Content references (evidence mapping)

- `EvidenceMappingSO` - Maps `trace_id` to score deltas and UI labels
  - Defines `displayName`/`description` for reporting
  - Applies multi-score impacts for each trace

**File Count:** 2 files
**Dependencies:** None (pure data)
**Dependents:** `App`, `Trace`, `Feedback`

---

### Level 4: Runtime & Interactivity (`SeriousGame.Runtime`)

**Purpose:** Core game mechanics and interactive objects

**Key Classes:**

- `IInteractable` - Interface for interactive game objects
  - Single method: `Interact()`

- `NodeInteractable` (MonoBehaviour) - Triggers dialogue
  - Implements `IInteractable`
  - Starts Yarn node on interaction

- `PlayerController` (MonoBehaviour) - Player character control
  - Handles movement and rotation
  - Receives input from `NarrativeService`
  - `SetLockState()` - Lock/unlock during dialogue

**File Count:** 2 interfaces + implementations
**Dependencies:** `App` (GameBootstrap context)
**Dependents:** `Narrative`

---

### Level 5: Input & Interaction (`SeriousGame.Interactables`)

**Purpose:** Specific interactive objects in the game world

**Key Classes:**

- `StateFlagInteractable` (MonoBehaviour)
  - Sets player flags on interaction
  - Implements `IInteractable`

- `TeleportInteractable` (MonoBehaviour)
  - Teleports player to location
  - Implements `IInteractable`

**File Count:** 2 files
**Dependencies:** None (direct MonoBehaviour implementation)
**Dependents:** None

---

### Level 6: Authentication (`SeriousGame.Auth`)

**Purpose:** User authentication, primarily Firebase

**Key Classes:**

- `AuthService` - Authentication manager
  - `LoginWithEmail()` - Firebase email authentication
  - Manages `IdToken`, `LocalId`
  - HTTP-based Firebase REST API integration

**File Count:** 1 file
**Dependencies:** Firebase Web API
**Dependents:** `UI` (LoginUI)

---

### Level 7: UI & Presentation (`SeriousGame.UI`)

**Purpose:** User interface management and event handling

**Key Classes:**

- `UIFlowManager` (MonoBehaviour) - Scene transitions
  - `StartGame()` - Load episode
  - `QuitToMenu()` - Return to main menu
  - `LoadScene()` - Generic scene loading

- `LoginUI` (MonoBehaviour) - Authentication UI
  - Login form interface

- `OutcomeToastUI` (MonoBehaviour) - Toast notifications
  - Display temporary feedback messages

- `ToastUIListener` - Event listener for toast events
  - Responds to `GameEventBus.OnToastRequested`

- `ScreenFader` (MonoBehaviour) - Screen fade effects
  - Scene transition visual effects

- `BillboardToCamera` (MonoBehaviour) - UI orientation
  - Keeps UI elements facing camera

**File Count:** 6 files
**Dependencies:** `App` (AppContext, GameBootstrap)
**Dependents:** None

---

### Level 8: Supporting Systems

#### **SeriousGame.Feedback** (covered above)

#### **SeriousGame.Phone** - Phone UI System

**Key Classes:**

- `PhoneUIListener` - Phone interface event handler
  - Responds to `GameEventBus.OnPhoneChatRequested`
  - Responds to `GameEventBus.OnPhoneMessageReceived`

**File Count:** 1 file
**Dependencies:** `App` (GameEventBus)

---

## Architectural Patterns & Principles

### 1. **Dependency Injection via Service Container**

```csharp
// AppContext serves as central DI container
public class AppContext : MonoBehaviour
{
    public AppConfigSO Config { get; private set; }
    public TraceService Trace { get; private set; }
    public PlayerStateService PlayerState { get; private set; }
    public SaveService Save { get; private set; }
    // ... other services
}
```

**Benefits:**

- Loose coupling between services
- Easy testing and mocking
- Single point of service initialization
- Clear dependency visibility

---

### 2. **Event Bus for Decoupled Communication**

```csharp
// Static event pattern for UI/system events
public static class GameEventBus
{
    public static event Action<string> OnPhoneChatRequested;
    public static event Action OnSummaryRequested;
    // ...
}
```

**Benefits:**

- Avoids circular dependencies
- UI components don't need references to core systems
- Events are discoverable (all in one place)

---

### 3. **Strategy Pattern for Trace Storage**

```csharp
public interface ITraceStore
{
    void Add(GameTrace trace);
    List<GameTrace> GetBySession(string sessionId);
    void ClearSession(string sessionId);
}

// Multiple implementations:
- InMemoryTraceStore
- FileTraceStore
- CloudTraceStore
```

**Benefits:**

- Switch implementations without changing code
- Easy to add new storage backends
- Decoupled from storage specifics

---

### 4. **Singleton Pattern for Bootstrap**

```csharp
public class GameBootstrap : MonoBehaviour
{
    public static AppContext Context { get; private set; }
    // Singleton initialization and DontDestroyOnLoad
}
```

**Benefits:**

- Global access to services
- Single initialization point
- Survives scene reloads

---

### 5. **Factory Pattern Implicit in TraceService**

```csharp
// TraceService creates GameTrace instances
public GameTrace BuildTrace(...) { ... }
```

---

### 6. **State Snapshot Pattern (Memento)**

```csharp
// PlayerStateSnapshot captures state at moment in time
[Serializable]
public class PlayerStateSnapshot
{
    public List<PlayerStateEntry> entries;
    public List<PlayerFlagEntry> flags;
}
```

**Benefits:**

- Non-destructive state captures
- Enable undo/redo functionality
- Persistent state management

---

## Data Flow Diagrams

### Game Initialization Flow

```
GameBootstrap.Awake()
    ↓
Create AppContext
    ↓
AppContext.Init(AppConfigSO)
    ├─ Create SceneService
    ├─ Create SessionService → Begin() → Generate sessionId
    ├─ Create PlayerStateService → Initialize defaults
    ├─ Create TraceService with InMemoryTraceStore + FileTraceStore
    ├─ Create AuthService
    ├─ Create NarrativeService
    ├─ Create FeedbackService
    └─ Create SaveService
    ↓
AppContext assigned to GameBootstrap.Context (static)
    ↓
Scene Ready (ready for gameplay)
```

---

### Player Action Trace Flow

```
Player Action (e.g., dialogue choice)
    ↓
YarnCommandBridge / NodeInteractable
    ↓
TraceService.BuildTrace()
    ├─ Reads: SessionService.CurrentSessionId
    ├─ Reads: PlayerStateService current state
    ├─ Creates: GameTrace object
    └─ Returns: Populated GameTrace
    ↓
TraceService.Add(GameTrace)
    ├─ InMemoryTraceStore.Add() [fast access]
    └─ FileTraceStore.Add() [persistence]
    ↓
Async: TraceService.SendSessionData() → Cloud persistence
```

---

### Save/Load Flow

```
Save Game
    ↓
SaveService.BuildCurrent()
    ├─ SessionService → session context
    ├─ PlayerStateService → state snapshot
    ├─ NarrativeService → current node/milestone
    └─ Creates: SaveData object
    ↓
SaveService.Save(SaveData)
    ↓
Serialize to JSON
    ↓
Write to: Application.persistentDataPath/save.json

---

Load Game
    ↓
SaveService.Load()
    ↓
Read JSON from persistentDataPath
    ↓
Deserialize → SaveData object
    ↓
Update PlayerStateService with loaded state
    ↓
Update NarrativeService with current node
    ↓
Load Unity scene from SaveData.currentUnityScene
```

---

### Event Bus Communication Flow

```
UI Component needs to show toast
    ↓
GameEventBus.RaiseToastRequested(message)
    ↓
OnToastRequested event fires
    ↓
ToastUIListener.OnToastRequested(message)
    ↓
OutcomeToastUI displays message
```

---

## Class Dependency Matrix

| From             | To                 | Type        | Notes              |
| ---------------- | ------------------ | ----------- | ------------------ |
| AppContext       | All Services       | Composition | DI Container       |
| GameBootstrap    | AppContext         | Composition | Initialization     |
| TraceService     | ITraceStore        | Interface   | Strategy Pattern   |
| TraceService     | SessionService     | Dependency  | Session context    |
| TraceService     | PlayerStateService | Dependency  | State snapshot     |
| SaveService      | PlayerStateService | Dependency  | State persistence  |
| SaveService      | SessionService     | Dependency  | Session context    |
| SaveService      | NarrativeService   | Dependency  | Narrative context  |
| FeedbackService  | TraceService       | Dependency  | Event analysis     |
| FeedbackService  | EvidenceMappingSO  | Dependency  | Trace definitions  |
| FeedbackService  | PlayerStateService | Dependency  | State analysis     |
| NarrativeService | PlayerController   | Dependency  | Input control      |
| NarrativeService | GameBootstrap      | Dependency  | Context access     |
| UIFlowManager    | GameBootstrap      | Dependency  | Service access     |
| NodeInteractable | GameBootstrap      | Dependency  | Context access     |
| AuthService      | Firebase API       | External    | HTTP-based         |
| UI Components    | GameEventBus       | Observer    | Event subscription |

---

## Key Design Decisions

### 1. **Service Container Over DI Framework**

- Manual DI through `AppContext` rather than a full DI container
- Pros: No external dependencies, explicit dependencies, Unity-friendly
- Cons: Manual management, no automatic injection

### 2. **Static GameEventBus**

- Event dispatcher for UI communication
- Avoids circular dependencies
- UI doesn't need AppContext reference

### 3. **Dual Trace Storage**

- In-memory for quick gameplay access
- File storage for persistence
- Enables future cloud sync without changing trace API

### 4. **MonoBehaviour vs Pure Services**

- `NarrativeService`: MonoBehaviour (integrates with Yarn Spinner)
- Most Services: Pure C# classes (more testable)
- Interactables: MonoBehaviour (world objects)

### 5. **State Snapshot Instead of Direct State Query**

- Captures state at specific moments
- Enables time-travel debugging, analytics
- Decouples save/trace systems from current state

---

## External Dependencies

| Package                         | Usage                         | Location                             |
| ------------------------------- | ----------------------------- | ------------------------------------ |
| **Yarn Spinner**                | Dialogue engine               | `NarrativeService`, `DialogueRunner` |
| **Firebase**                    | Authentication, Cloud Storage | `AuthService`, `CloudTraceStore`     |
| **Newtonsoft.Json**             | JSON serialization            | `AuthService`, Trace storage         |
| **UnityEngine.Networking**      | HTTP requests                 | `AuthService`                        |
| **UnityEngine.SceneManagement** | Scene loading                 | `SceneService`, `UIFlowManager`      |

---

## File Statistics

| Namespace     | File Count | Purpose                          |
| ------------- | ---------- | -------------------------------- |
| App           | 6          | Initialization, services, events |
| State         | 4          | Player state management          |
| Trace         | 6          | Event logging and analytics      |
| Trace.Dto     | 3          | Data models                      |
| Save          | 2          | Persistence                      |
| Narrative     | 3          | Story/dialogue integration       |
| Feedback      | 2          | Analytics reports                |
| UI            | 6          | UI management                    |
| Auth          | 1          | Authentication                   |
| Content       | 3          | Configuration                    |
| Runtime       | 2          | Core interfaces                  |
| Interactables | 2          | Interactive objects              |
| Phone         | 1          | Phone UI                         |
| Player        | 1          | Player controller                |
| **Total**     | **42**     | **Complete codebase**            |

---

## Architecture Patterns Summary

```
┌─────────────────────────────────────────────┐
│           Unity Application                  │
└─────────────────────────────────────────────┘
                    ↑
                    │ DontDestroyOnLoad
                    │
        ┌───────────────────────────┐
        │    GameBootstrap           │
        │   (Singleton Entry)        │
        └───────────────────────────┘
                    ↓
        ┌───────────────────────────────────┐
        │      AppContext (Service Hub)     │
        ├───────────────────────────────────┤
        │ • SessionService                  │
        │ • PlayerStateService              │
        │ • TraceService                    │
        │ • SaveService                     │
        │ • NarrativeService                │
        │ • FeedbackService                 │
        │ • AuthService                     │
        │ • SceneService                    │
        │ • AppConfigSO                     │
        └───────────────────────────────────┘
         ↑         ↑         ↑         ↑
         │         │         │         │
    ┌────┴─┐  ┌────┴─┐  ┌────┴─┐  ┌──┴──┐
    │ Game │  │  UI  │  │Auth  │  │Nar  │
    │Logic │  │Layer │  │Layer │  │Layer│
    └──────┘  └──────┘  └──────┘  └─────┘
         ↓         ↓         ↓         ↓
    ┌────────────────────────────────────┐
    │   GameEventBus (Decoupled Events)  │
    └────────────────────────────────────┘
```

---

## Summary

The **Serious Game Scam** codebase follows a **layered service-oriented architecture** with:

1. **Clear Separation of Concerns** - Each namespace handles a specific domain
2. **Dependency Injection** - Services injected through AppContext
3. **Event-Driven UI** - GameEventBus prevents tight coupling
4. **Strategy Pattern** - Multiple trace storage implementations
5. **Data Models** - Clean DTOs for serialization
6. **Extensibility** - Easy to add new services, trace types, or storage backends

The architecture supports:

- ✅ Educational content delivery via Yarn dialogues
- ✅ Event tracking for learning analytics
- ✅ Save/load game progress
- ✅ Feedback generation based on player actions
- ✅ Authentication and user management
- ✅ Modular UI with clean event communication
