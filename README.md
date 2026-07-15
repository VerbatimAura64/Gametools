# GameTools

Reusable Unity package containing a first-person character controller and an editor-time population/hide-and-seek spawner tool. Built originally for a Synty 12th Birthday Jam entry, packaged for reuse across future prototypes and jams.

## Contents

- **Character Controller** (`Runtime/CharacterController/`) — first-person movement, mouse look, sprint, gravity, and animation blend-tree driving via the new Input System.
- **Population Spawner** (`Runtime/PopulationSpawner/`) — an editor tool that populates a defined zone with decoy NPCs and one hidden target, tunable by a camouflage-tightness value. Includes raycast-based interaction for the player to guess/find the target.

## Requirements

- Unity's new **Input System** package
- **TextMeshPro** (used by the fragment display)
- URP (the population spawner's camouflage tint uses `_BaseColor`; swap to `_Color` in `TargetTell` if using Built-in Render Pipeline)

## Setup

### 1. Implement the two interfaces

This package doesn't assume a specific game manager or UI system — it expects the consuming project to provide two small interfaces on a GameObject tagged **`GameController`**:

```csharp
public interface IGameState
{
    bool IsPaused { get; }
    void PauseGame();
}

public interface IFragmentDisplay
{
    GameObject Panel { get; }
    void ClosePanel();
    void Unlock(StoryFragment fragment);
}
```

Your project's own `GameManager` and a fragment/UI display script should implement these. Both `InputManager` and `ZoneMarker` fetch them via `GetComponent<T>()` on the `GameController`-tagged object in `Awake()` — no static singletons required.

> Note: if using explicit interface implementation (`bool IGameState.IsPaused => ...`), double check it forwards to a real backing property rather than being left as an IDE-generated stub — a leftover `throw new NotImplementedException()` is a common gotcha here.

### 2. Character Controller

Add `InputManager` to your player GameObject (requires a `CharacterController` component, added automatically via `[RequireComponent]`). Configure in the Inspector:

- `playerSpeed`, `sprintMultiplier`, `gravity`, `jumpHeight`, `sensitivity`, `dampTime`
- Requires Input Actions named `Move`, `Attack`, `Pause`, `Next`, and `Sprint` to exist in your project's Input Actions asset
- Animator expects two floats, `walkingX` and `walkingY`, feeding a 2D Freeform Directional blend tree

Camera should be a **child of the player object** — the controller applies yaw to the player body and pitch to the camera separately, so they must be parented for this split to work correctly.

### 3. Population Spawner

1. Create one or more **`PopulationPool`** assets (`Create > HarborView.GameTools > Population Pool`). Each pool needs a list of weighted decoy prefabs and one target prefab.
2. Every decoy and target prefab needs an **`Interactable`** component. The target prefab additionally needs a **`TargetTell`** component (references its own `Renderer`) for the camouflage tint.
3. Create a **`StoryFragment`** asset per zone (`Create > HarborView.GameTools > Story Fragment`) — holds the text/order data unlocked when that zone's target is found.
4. Add a **`ZoneMarker`** component to an empty GameObject in your scene. Assign its `Pool`, `Fragment`, a `Camouflage Tightness` value (0 = obvious, 1 = blended in), and a set of `Spawn Points` (child Transforms placed where NPCs should land).
5. Use the **Populate** / **Clear** buttons in the `ZoneMarker`'s Inspector (via the custom editor) to generate or reset the zone's NPCs.

Player interaction is raycast-based: pressing the `Attack`/interact action fires a raycast from screen center; hitting an `Interactable` calls `OnInteract()`, which resolves correct/incorrect against the zone via its `ownerZone` reference.

## Known Limitations / Future Work

- The random spawn-point generator (`CreateSpawns`) is unweighted and has no collision/overlap avoidance — fine for rough prototyping, not for final content placement.
- Camouflage tightness currently only blends a single material color tint (via `MaterialPropertyBlock`); no animation-based tell variation yet.
- No guess-cost/failure-state system (unlimited free guesses per zone) — flagged as a stretch goal, not yet implemented.
- Namespace coverage is inconsistent — `IGameState`/`IFragmentDisplay` are wrapped in `HarborView.GameTools`, other package types are currently in the global namespace. A full namespace pass is a planned cleanup.

## Folder Structure

```
Runtime/
├── CharacterController/
│   └── InputManager.cs
└── PopulationSpawner/
    ├── ZoneMarker.cs
    ├── PopulationPool.cs
    ├── Interactable.cs
    ├── TargetTell.cs
    ├── StoryFragment.cs
    ├── IGameState.cs
    └── IFragmentDisplay.cs
Editor/
└── ZoneMarkerEditor.cs
```