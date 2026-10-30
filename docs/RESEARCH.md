# Vanilla research notes

## Implementation evidence — 2026-10-29

This development pass also inspected the exact installed 1.22.7 managed assemblies and API XML, using read-only metadata/IL inspection. The game runtime configuration targets .NET 10; local runtime 10.0.7 is available. Public master source remains separate evidence.

- `SystemTemporalStability.StormData` exposes storm activation, strength, expiry and next-storm identity. The adapter postpones active expiry and invokes the installed `onTempStormTick` expiry path on final death/abandonment.
- Rift candidates use the installed `ModSystemRifts.canSpawnRiftAt` hook plus additional ground/headroom checks; the owned visual Rift is saved and removed independently. Failure skips the encounter rather than claiming an arbitrary position is vanilla-compatible.
- `EntityBehaviorTemporalStabilityAffected` interprets values below one as stability drain. Multiplying the difference between storm and nonstorm stability can incorrectly include lost positive recovery; the core helper instead scales incremental negative drain. Quantitative regression cases cover this distinction.
- `EntityProperties.Clone` does not preserve initialized behavior instances. Guard protection restores the entity's existing health/AI behavior objects on its isolated clone, while removing harvest/capture behavior and rewards. Reload reapplies guard protection by persisted provenance, including survivors from previous encounters.
- Current storm stability callbacks can conflict with later subscribers from other mods; base-game-only compatibility is the present target. No general third-party transformation, capture or stability-mod compatibility is established.

Compiled bindings, static inspection and isolated server startup are different evidence levels from interactive gameplay. Current results are tracked in [CURRENT_CONTEXT.md](CURRENT_CONTEXT.md), with runtime gates in [PLAYTEST.md](PLAYTEST.md).

## Earlier asset/source research

For narrative research begun 2026-10-24, see [the lore ledger and roadmap](LORE_RESEARCH.md) and [source coverage](LORE_SOURCE_COVERAGE.md). Those distinguish direct game statements, character testimony, inference, unresolved evidence, community interpretations, and original mod additions. The mechanical investigation below remains separate; it does not establish creature origins.

Read-only investigation performed 2026-10-24. No game assets, settings, saves, or running behavior were changed.

## Evidence and version limits

The installed client log identified **Vintage Story 1.22.7 (Stable)** at `E:\Games\Vintagestory\Vintagestory.exe`. The base assets inspected were:

- `assets/survival/entities/lore/bell.json`
- `assets/survival/entities/lore/drifter.json`
- `assets/survival/entities/lore/bowtorn.json`
- `assets/survival/entities/lore/shiver.json`
- `assets/survival/config/mobextraspawns.json`

These are base assets, not the final mod-patched asset state. Anego's public master source was inspected separately; it may differ from the installed DLLs. No decompilation, gameplay experiment, or effective configuration audit was performed.

## Bell configuration in installed base assets

| Setting | Value |
|---|---|
| HP | 100 |
| Reinforcement quantity setting | 6 |
| Interval | 1,000–5,000 ms |
| Spawn range setting | 12 blocks |
| Seeking range setting | 15 blocks |
| Despawn | minPlayerDistance 30; minSeconds 900 |
| Reinforcement list | drifter-deep; drifter-tainted (twice); drifter-corrupt; shiver-deep; bowtorn-deep; locust-corrupt |

The duplicated tainted entry weights the list; it is not two simultaneous spawns. Bell's ordinary natural-spawn config also has maxQuantity 3, maxLightLevel 5, chance 0.0018, maxY 0.7, and an air-block requirement. Those natural-spawn settings are distinct from its reinforcement task and from rift placement rules.

## Published bell alarm code

[AiTaskBellAlarm.cs](https://github.com/anegostudios/vssurvivalmod/blob/master/Entity/AITask/AiTaskBellAlarm.cs)

- Keeps its own `spawnedEntities` list; dead entries are removed before reinforcement attempts.
- Spawns at most one creature per successful interval check, with randomized placement/collision attempts.
- Uses nearby living player count and `SpawnCapPlayerScaling` to calculate allowance.
- The published comparison is `count <= allowance`, permitting one above that calculated allowance. Do not copy this if a strict event cap is required.
- Detecting a player depends on activity, sneaking, distance, and listening state, not only the nominal seeking radius.
- Tags spawned entities with origin `bellalarm`; this is useful provenance, not a complete persisted event-ownership system.

Implication: sharing ownership/cap/pause across four chanters and an actual bell requires custom coordination. Four independent stock bell tasks would each maintain a separate allowance. Stock bell reinforcements would also bypass specialist-family shutdown unless deliberately coordinated.

## Published storm spawning code and local settings

[TemporalStability.cs](https://github.com/anegostudios/vssurvivalmod/blob/master/Systems/TemporalStability/TemporalStability.cs)

- Separate storm spawner with a nominal 2-second system tick, random skips, and spawn breaks.
- Tries positions around each player using a range setting of 15; counts relevant nearby groups within range + 30 (45 blocks).
- Group allowance uses `round((2 + stormStrength * 8) * groupWeight)`, then the family quantity multiplier.
- Local default weights: drifter 0.5, shiver 0.25, bowtorn 0.25; family quantity multipliers 1, 0.75, 0.75.
- Heavy-strength defaults yield approximate group targets of 5 drifters, 2 shivers, and 2 bowtorns. Batch placement can overshoot a target; rare variants and other patterns further change actual composition.
- Local config has alternative family-heavy patterns and rare drifter-double-headed / shiver-deepsplit entries.
- Placement uses ground search and collision clearance, not the normal light-level checks.
- Storm termination includes spawn/despawn/daylight bookkeeping. Ending a storm correctly requires examining that lifecycle, not simply changing a visible effect.

These rules describe storm mob spawning. **The actual temporal-rift placement implementation has not been inspected yet.**

## Health and stability balance examples

Installed base max HP: drifter normal 12, deep 16, tainted 22, corrupt 30, nightmare 40, double-headed 54; bowtorn gearfoot 50; shiver deepsplit 60, bellhead 66.

Bowtorn base on-death stability recovery is 0.30 for nightmare and 0.35 for gearfoot. Replenishing high-tier enemies can yield substantial stability and loot, so spawn delay alone does not settle balance.

## Other references

- [Bell wiki](https://wiki.vintagestory.at/Bell/en): page states verification for 1.19.4, so its roster is not authoritative for 1.22.7.
- [Storm wiki](https://wiki.vintagestory.at/Temporal_storms): high-level overview.
- [Entity tutorial](https://wiki.vintagestory.at/Modding:Content_Tutorial_Entity_Creation_Part_2): behavior/AI/spawning introduction, versioned separately.

No live test established final cap behavior, performance, multiplayer scaling, or compatibility with the user's mods. Published source links track master and can change.
