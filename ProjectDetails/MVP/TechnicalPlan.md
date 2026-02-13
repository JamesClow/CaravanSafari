## Technical Plan: The Last Caravan — MVP Implementation

### What You Have Today

**Unity Project** (Unity 6, with NavMesh AI Navigation package)

**14 Scripts — most are stubs or early prototypes:**

| Script | Status | Notes |
|---|---|---|
| `SingletonMonoBehaviour.cs` | ✅ Done | Generic singleton base — solid, keep as-is |
| `AudioManager.cs` | ✅ Done | Working audio system with randomized pitch — good to go |
| `Health.cs` | ✅ Done | Health/damage/death — works, needs minor expansion (events) |
| `Targeting.cs` | ✅ Done | Trigger-based detection by tag — usable for towers and units |
| `Launcher.cs` | ✅ Done | Fires projectiles on interval at a target — works |
| `Projectile.cs` | ✅ Done | Homing projectile — works |
| `Enemy.cs` | ⚠️ Partial | Navigates to HomeBase, takes damage — needs rework for moving base |
| `LevelManager.cs` | ⚠️ Partial | Wave/Wavelet spawning — needs major rework for day/night cycle |
| `HomeBase.cs` | 🔴 Stub | Empty singleton — needs to become the moving caravan |
| `Attack.cs` | 🔴 Stub | Just a field, no logic |
| `Tower.cs` | 🔴 Stub | Empty |
| `Spawner.cs` | 🔴 Stub | Empty |
| `NavUnitTest.cs` | 🧪 Test | Click-to-move test — useful reference, not production |
| `ObstacleAnimation.cs` | 🧪 Test | Sine-wave animation — test only |

**Prefabs:** Enemy, HomeBase, Missle, NavPlayer, NavWall, Obstacle, Spawner, Tower
**Scenes:** EnemyTesting, GridTesting, NavTesting (all test scenes)
**Assets:** Materials for ground/player/obstacle/missile/walls, large SFX library, one FBX model

---

### What the MVP Requires (from CoreConceptsMVP.md)

Six core systems and basic content:

1. **Hero Unit** — directly controllable, camera follows
2. **Moving Base** — continuous movement along a path
3. **Unit Calling System** — hold button to summon nearby neutral units
4. **Unit Collection** — bring called units to base to capture them
5. **Day/Night Cycle + Deterministic Wave System** — waves trigger at night
6. **Automatic Unit Combat** — captured units fight during night phase

---

### Technical Implementation Plan

I'd recommend building this in **5 phases**, each one producing a playable slice you can test before moving on.

---

#### Phase 1 — Hero Controller & Camera (Foundation)

**Goal:** Walk around a map with a controllable hero and a following camera.

**New scripts:**
- `HeroController.cs` — WASD/gamepad movement using CharacterController or NavMeshAgent, handles input for movement and the "call" action
- `HeroCameraController.cs` — Camera that follows hero with smooth damping (offset-based third-person or top-down)

**Rework:**
- `NavUnitTest.cs` → retire or keep for debug; hero uses direct input, not click-to-move

**Prefab:**
- Create a **Hero** prefab (reuse NavPlayer mesh/material, add `HeroController`, `Health`, `HeroCameraController`)

---

#### Phase 2 — Moving Base (The Core Hook)

**Goal:** A home base that moves continuously along a predefined path. Units can rally to it.

**New scripts:**
- `CaravanPath.cs` — Defines the path as a series of waypoints (array of Transforms or a Spline). Exposes current progress, direction, and speed
- `MovingBase.cs` — Replaces current empty `HomeBase.cs`. Follows `CaravanPath` at a configurable speed. Exposes a "rally point" (transform near the base where captured units gather). Singleton so enemies and units can reference it

**Rework:**
- `HomeBase.cs` → becomes `MovingBase.cs` (or HomeBase gains movement logic)
- `Enemy.cs` → currently does `agent.SetDestination(HomeBase.position)` once in Start. Needs to **continuously** update destination to track the moving base (do it in Update on an interval, or subscribe to a base-moved event)

**Scene work:**
- Build a test scene with a ground plane and a series of waypoint empties defining the caravan path
- Place the HomeBase prefab on the path

---

#### Phase 3 — Unit Calling & Collection (The Pikmin Loop)

**Goal:** Neutral units exist on the map. Hero calls them, leads them back, and captures them at the base.

**New scripts:**
- `NeutralUnit.cs` — State machine with states: **Idle** (wanders/stands still on map), **Following** (follows hero via NavMeshAgent), **Captured** (belongs to base, fights at night). Handles transitions between states
- `UnitCallSystem.cs` — Component on Hero. When "call" button is held, finds all NeutralUnits within a radius (Physics.OverlapSphere + tag/layer check), tells them to enter Following state. Visual/audio feedback (radius indicator, call sound)
- `UnitCaptureZone.cs` — Trigger collider on/near the moving base. When a Following unit enters the zone, it transitions to Captured state and is added to the base's army roster

**New data:**
- `UnitStats` ScriptableObject (or simple serializable class) — health, damage, attack speed, movement speed. Allows different neutral unit types

**Prefabs:**
- **NeutralUnit** prefab — mesh + NavMeshAgent + `NeutralUnit.cs` + `Health` + `Targeting` + `Attack`
- Variants for 2–3 basic unit types (melee, ranged, maybe a tanky one)

**Rework:**
- `Attack.cs` → implement actual attack logic (timer → find target from `Targeting` → deal damage or launch projectile). This powers both captured friendly units and towers

---

#### Phase 4 — Day/Night Cycle & Wave System

**Goal:** Time cycles between day (explore/collect) and night (defend). Waves are deterministic and escalating.

**New scripts:**
- `DayNightCycle.cs` — Singleton manager. Tracks elapsed time, transitions between Day and Night phases. Fires events: `OnDayStarted`, `OnNightStarted`, `OnPhaseTimeUpdate`. Controls directional light rotation/color and ambient lighting for visual feedback. Configurable day length and night length
- `WaveManager.cs` — Replaces/reworks `LevelManager.cs`. Listens for `OnNightStarted`, triggers the next deterministic wave. Each wave is a `WaveDefinition` ScriptableObject containing a list of spawn groups (enemy prefab, count, spawn direction/point, delay). Tracks active enemies; fires `OnWaveComplete` when all are dead. Escalates wave index each night

**New data:**
- `WaveDefinition` ScriptableObject — enemy composition, timing, spawn positions (relative to base or absolute)
- Consider spawn points that move WITH or relative to the caravan (e.g., "spawn 50 units north of current base position")

**Rework:**
- `LevelManager.cs` → retire in favor of `WaveManager.cs` (or gut and rewrite). The current Wavelet system is a decent start but needs to tie into day/night events and support relative spawn positions
- `NeutralUnit.cs` → Captured units switch behavior on `OnNightStarted` (start fighting) and `OnDayStarted` (return to base/idle)
- `Enemy.cs` → ensure enemies continuously chase the moving base

**Scene work:**
- Lighting setup: directional light that rotates/changes color based on cycle
- UI: simple day/night indicator + wave counter

---

#### Phase 5 — Automatic Unit Combat & Game Loop Polish

**Goal:** Captured units fight enemies automatically during night. Tie the full loop together.

**Rework:**
- `Attack.cs` → fully implemented attack system. Timer-based, uses `Targeting` to find nearest enemy, deals damage (melee range check or spawns `Projectile`)
- `NeutralUnit.cs` in Captured state → during night, uses NavMeshAgent to move toward nearby enemies (via `Targeting`), attacks with `Attack.cs`. During day, follows the caravan
- `Health.cs` → add an `OnDeath` event (C# event/UnityEvent) so `WaveManager` can track enemy kills and other systems can react

**New scripts:**
- `ArmyManager.cs` — Tracks all captured units. Provides count for UI. Handles unit positioning around the caravan (formation or simple follow). Can be a component on the MovingBase
- `GameManager.cs` — Singleton orchestrating the full game loop: Start → Day → Night → Wave → Check win/lose → Repeat. Win condition: reach end of path. Lose condition: base health reaches 0
- `UIManager.cs` — Minimal HUD: wave counter, day/night indicator, unit count, base health bar, call radius indicator

**Prefabs:**
- Update **Enemy** prefab with variants (basic melee, ranged, fast) for wave variety

---

### Script Dependency Map

```
GameManager (orchestrates everything)
├── DayNightCycle (time, phase events, lighting)
├── WaveManager (deterministic waves, enemy spawning)
│   └── Enemy (NavMeshAgent → MovingBase, Health, Attack)
├── MovingBase / HomeBase (path following, singleton)
│   ├── CaravanPath (waypoints)
│   ├── UnitCaptureZone (trigger for capturing units)
│   └── ArmyManager (tracks captured units)
├── HeroController (player input, movement)
│   ├── HeroCameraController (camera follow)
│   └── UnitCallSystem (radius call mechanic)
└── NeutralUnit (Idle → Following → Captured state machine)
    ├── Health
    ├── Targeting
    └── Attack (shared with towers/enemies)
```

### Reuse Summary

| Existing | Action |
|---|---|
| `SingletonMonoBehaviour` | Keep as-is ✅ |
| `AudioManager` + `Sound` | Keep as-is ✅ |
| `Health` | Extend with `OnDeath` event |
| `Targeting` | Keep as-is, used by units + towers ✅ |
| `Launcher` | Keep for ranged units/towers ✅ |
| `Projectile` | Keep as-is ✅ |
| `HomeBase` | Rework → `MovingBase` with path following |
| `Enemy` | Rework → continuous destination updates |
| `LevelManager` | Rework → `WaveManager` tied to day/night |
| `Attack` | Implement fully (timer + targeting + damage) |
| `Tower` | Implement later or repurpose for captured ranged units |
| `Spawner` | Retire (absorbed by `WaveManager`) |
| `NavUnitTest` | Retire (replaced by `HeroController`) |
| `ObstacleAnimation` | Retire or keep for decorative use |

### Recommended Build Order

| Order | Phase | Validates |
|---|---|---|
| 1 | Hero + Camera | "Does it feel good to move around?" |
| 2 | Moving Base + Path | "Does defending a moving target feel different?" |
| 3 | Unit Calling + Collection | "Is the Pikmin-style collection loop satisfying?" |
| 4 | Day/Night + Waves | "Does the day/night pressure create real tension?" |
| 5 | Auto Combat + Game Loop | "Does the full loop work end to end?" |

Each phase produces something playable. If any phase doesn't feel fun, you can iterate on it before adding more complexity — exactly what the design feedback recommended.
