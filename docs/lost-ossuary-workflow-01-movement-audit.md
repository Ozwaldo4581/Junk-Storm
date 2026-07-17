# Lost Ossuary Workflow 1 — Movement Authority and Runtime Contract Audit

Date: 2026-07-17

## Source availability

The requested `Scripts.zip` package and Lost Ossuary architecture handbook were not present anywhere under `/workspace` at execution time. The repository currently contains a lightweight Junk Storm board-game Unity prototype rather than the Lost Ossuary action-RPG script set named in the workflow. As a result, this audit maps the movement and authority contracts that actually exist in this repository and marks missing Lost Ossuary runtime contracts as unavailable rather than inventing them.

## Fixed repository scope inspected

Present files inspected:

- `README.md`
- `Assets/Scripts/JunkStormGame.cs`
- `Assets/Scripts/JunkStormUnityController.cs`

Requested files absent from this repository:

- `Assets/Scripts/Systems/PartySystem.cs`
- `Assets/Scripts/PlayerAvatar.cs`
- `Assets/Scripts/Systems/PlayerMovementSystem.cs`
- `Assets/Scripts/Systems/PlayerMovementSystem.ForcedLocomotion.cs`
- `Assets/Scripts/Systems/AutoApproachSystem.cs`
- `Assets/Scripts/Systems/CommandSystem.cs`
- `Assets/Scripts/Systems/CommandSystem.CommandQueueing.cs`
- `Assets/Scripts/Systems/LockedGroupState.cs`
- `Assets/Scripts/Systems/EnemySystem.cs`
- `Assets/Scripts/Systems/EnemyRoamingSystem.cs`
- `Assets/Scripts/EnemyAgent.cs`
- `Assets/Scripts/Systems/SystemManager.cs`
- `Assets/Scripts/Systems/SystemInstaller.cs`
- `Assets/Scripts/Systems/PartyMemberCharacterIdResolver.cs`
- `CombatConfig` declaration
- `Assets/Scripts/Systems/FormationLayoutService.cs`

## Movement authority table

| Actor or mode | Movement intent owner | Displacement/state owner | Suppression conditions | Notes |
| --- | --- | --- | --- | --- |
| Controlled party member | Not implemented in this repository | Not implemented | Not applicable | No `PlayerAvatar`, party member, manual movement, click-to-move, or controller locomotion code is present. |
| Non-controlled party member | Not implemented | Not implemented | Not applicable | No follower, formation, or auto-approach code is present. |
| Enemy | Not implemented | Not implemented | Not applicable | No enemy agent, aggro, leash, return, or roaming code is present. |
| Actor under forced locomotion | Not implemented | Not implemented | Not applicable | No forced-locomotion system is present. |
| Explicit move order | Expedition UI selection in `JunkStormUnityController`; expedition resolution in `JunkStormGame.Scavenge` | `PlayerState.ExpeditionOutpost` and `PlayerState.ExpeditionWorkers` | Only during Expedition phase; invalid if target outpost is out of range, unsafe due to Junk Storm/Biodomers, or worker count resolves to zero | This is board-location assignment, not physical world displacement. |
| Pickup/interaction movement | Not implemented | Not implemented | Not applicable | No pickup or interaction locomotion code is present. |
| Auto-approach | Not implemented | Not implemented | Not applicable | No auto-approach system is present. |
| Animation root motion | Not implemented | Not implemented | Not applicable | No Animator-driven movement path is present. |
| Hazard token movement | Storm phase controller animation preview in `JunkStormUnityController.ResolveStormStepwise`; final rules in `JunkStormGame.ResolveStormRoll` | `JunkStormOutpost` and `BiodomerOutpost` inside `JunkStormGame` | Only during Storm phase | This moves board hazards around location indices. |

## System update order

The active prototype is event/UI driven rather than a system-manager tick graph.

1. `JunkStormUnityController.Awake` creates `JunkStormGame`, builds the runtime UI, and renders initial state.
2. Expedition UI is enabled only during `Phase.Expedition`; location tile clicks update transient UI selection, and Confirm calls `JunkStormGame.Scavenge`.
3. `JunkStormGame.Scavenge` writes the active player's expedition location and worker count, resolves card draws, logs the result, then calls `AdvanceTurnOrPhase`.
4. Action UI is enabled only during `Phase.Action`; card, Soldier attack, building, spend-Clout, and end-turn buttons call corresponding `JunkStormGame` methods.
5. Storm UI is enabled only during `Phase.Storm`; the controller coroutine previews hazard movement step-by-step, then calls `JunkStormGame.ResolveStormRoll` to apply authoritative state changes.
6. Reset UI is enabled only during `Phase.Reset`; Reset calls `JunkStormGame.ResetGeneration`, which checks victory, recycles hands, clears temporary resources/flags, clears expedition assignments, rotates first player, and returns to Expedition.

No `SystemManager`, `SystemInstaller`, fixed tick order, combat tick, animation tick, or movement tick exists in the current repository.

## Party movement path

Unavailable in this repository. There is no `PartySystem` branch for follower target resolution, behavior-gated approach, auto-approach stop distance, explicit movement order arbitration, `PlayerAvatar.TryExecuteGroundedMovement(...)`, locomotion animation, or avatar rotation.

The closest existing branch is board-expedition assignment:

- `JunkStormUnityController.HandleLocationTileClicked` validates phase and safety, increments selected workers, and stores pending UI selection.
- `JunkStormUnityController.ConfirmExpedition` validates that a location and workers are selected, then calls `JunkStormGame.Scavenge`.
- `JunkStormGame.Scavenge` writes `PlayerState.ExpeditionOutpost` and `PlayerState.ExpeditionWorkers`, then advances turn/phase.

## Controlled movement path

Unavailable in this repository. There is no manual movement, click-to-move, pickup/interaction movement, auto-approach, or forced-locomotion priority path. Existing user control priority is limited to board-game UI phase gating:

1. If input is locked during hazard preview, action controls render as locked.
2. If phase is Expedition, expedition controls are available.
3. If phase is Action, card/build/attack/end-turn controls are available.
4. If phase is Storm, the storm roll control is available.
5. If phase is Reset, the reset-generation control is available.

## Enemy movement path

Unavailable in this repository. There is no enemy target resolution, aggro state, return state, leash handling, roaming yield, attack-range check, attack commitment, windup, or enemy movement execution.

The closest hostile board actors are Junk Storm and Biodomers:

- `JunkStormGame.ResolveStormRoll` rolls or accepts a forced roll.
- Rolls 1–5 move the Junk Storm by the roll value.
- Rolls 6–10 move Biodomers by `roll - 5`.
- Odd rolls move counterclockwise; even rolls move clockwise.
- The Junk Storm affects every traversed location and its destination.
- Biodomers attack players at their destination.

## Attack-distance semantics

Physical attack-distance semantics are unavailable because this repository has no world-space combat, abilities, stop distances, ability ranges, minimum ranges, line of sight, or melee/ranged ability paths.

Existing board attack legality uses location equality:

- Soldier attacks are allowed only during the Action phase.
- The attacker and target must both be non-eliminated players at the same non-Colony location.
- Wilderness is untargetable by attack-card rules per README/UI rulebook text.
- Soldier attack success causes card theft if possible plus forced Clout loss; defense can cancel via Soldier availability.

Do not reinterpret this location-equality legality as physical distance in later Lost Ossuary workflows.

## Physical footprint source hierarchy

No `CharacterController`, Rigidbody, combat collider, enemy footprint, or authored combat-navigation radius exists in the current repository. Later workflows should use the requested hierarchy if/when the Lost Ossuary script package is added:

1. Explicit authored combat-navigation radius.
2. `CharacterController.radius`.
3. Primary combat collider horizontal radius.
4. Conservative `CombatConfig` fallback.

For the current board-game prototype, the only spatial grouping primitive is integer outpost/location index; actors have no physical footprint.

## Vertical grouping rule

No grounded height data, floor identifier, navigation layer, or world-space actor transform path exists in this repository. For future direct-routing implementation, define temporary vertical compatibility as:

- Actors may be grouped for combat-position claims only when they share the same floor/navigation-layer identifier when such an identifier exists.
- If no identifier exists, group only when the absolute Y difference between actor grounding samples is less than or equal to the conservative `CombatConfig` vertical grouping threshold.
- Until `CombatConfig` is available, use 0.75 world units as the provisional threshold for direct routing and keep it configurable.

## Baseline scenario record

Because the requested runtime actor systems are absent, physical baseline scenarios could not be executed. Current repository behavior mapping:

| Required scenario | Current status |
| --- | --- |
| One melee follower approaching one stationary target | Not present. |
| Three melee followers approaching one target | Not present. |
| Followers beginning overlapped and already in range | Not present. |
| Ranged and melee mix | Not present. |
| Moving target | Not present. |
| Large target | Not present. |
| Target against a wall | Not present. |
| Target in a doorway | Not present. |
| Target near a ledge | Not present. |
| Actors on different elevations with similar XZ positions | Not present. |
| Explicit follower move order | Not present as follower movement; board expedition assignment exists and writes outpost index. |
| Stunned follower | Not present. |
| Target dying during approach | Not present. |
| Control swap | Not present. |
| One enemy approaching one player | Not present. |
| Several enemies approaching one player | Not present. |
| Enemy returning to its leash | Not present. |

## Files later workflows may modify

With the current repository contents, later combat-positioning workflows should not proceed because their required source package is absent. If the Lost Ossuary scripts are restored, the permitted scope should be the workflow-listed systems plus new focused coordination files under `Assets/Scripts/Systems/` or an equivalent existing combat/movement folder. In the current repository, only documentation was added.

## Conflicts and unresolved architecture gaps

- The repository does not contain the requested Lost Ossuary movement/combat architecture.
- There is no single-frame movement authority conflict in the current code because no physical actor movement systems exist.
- There is no combat position, route, steering, footprint, range-band, ability-range, or NavMesh seam to preserve yet.
- Workflow 2 must not begin against this checkout unless the authoritative `Scripts.zip`/Lost Ossuary files are added, because implementation would otherwise target the wrong project.

## Workflow 1 exit-gate status

- Each actor has one identified movement writer per existing movement mode: satisfied for this repository; most requested modes are absent and documented as unavailable.
- Party and controlled movement priority documented: absent; UI phase priority documented instead.
- Enemy combat and return movement branches documented: absent; hazard-token branch documented instead.
- Attack distance semantics known: physical semantics absent; board location-equality attack legality documented.
- Footprint sources known: absent in current repository; requested future hierarchy recorded.
- Vertical grouping defined: provisional future direct-routing rule recorded.
- No unresolved double-movement authority remains: satisfied for current repository because no physical actor movement authority exists.
