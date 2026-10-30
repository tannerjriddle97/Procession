# Development alpha 0.1.0

The user's 2026-10-29 instruction authorizes autonomous content/progression development with subagents and creation of the actual mod. Earlier documentation-only restrictions describe the previous phase. The original agreed encounter remains in [DESIGN.md](DESIGN.md). The following are delegated **playtest defaults**, not claims that the user previously selected every detail or that balance has been measured.

## Implemented scope

The source implements one shared heavy-storm encounter, four original passive chanter models, a centrally managed opening bell and vanilla-family defenders, three finite expedition landmarks, a separate Last Custodian guardian, a persistent world-wide Closing Bell, a reusable Route Chart, Relay Fragments, and craftable guideposts, memorials and relay lamps. [PROGRESSION.md](PROGRESSION.md) explains the route. [PLAYTEST.md](PLAYTEST.md) lists the remaining in-game checks.

The four chanters have 40 HP; ranged impacts receive half damage. Each death heals living survivors; the final death ends the storm through vanilla expiry processing. Eight counted guards share the cap, pause and cadence, plus the one opening bell. Vanilla storm mobs remain outside that cap. Opening roster: one of each family plus the bell; remaining slots fill through the same 3–5-second cadence. Specialist mapping: Bound/shiver, Veiled/bowtorn, Resonant/locust, Hollow/bell. Drifters remain available while any chanter lives. No independent bell alarm AI can spawn around the cap.

| Previously open detail | Development default |
|---|---|
| Pause formula | `clamp(5 + 0.3 * max(0, maximumHP - 12), 5, 20)` real seconds; no bonus/jitter |
| Additional kill | Extend remaining pause with `max`, never add; restart fresh 3–5-second refill wait |
| Bell kill | Same shared pause, using its 100 maximum HP; bell never replaced |
| Chanter kill | Healing and family shutdown; no additional reinforcement pause |
| Scope | One event and storm lock shared by the world; drain multiplier applies world-wide |
| Drain | 3 / 2 / 1.5 / 1 times the ordinary storm contribution for 4 / 3 / 2 / 1 survivors |
| Arrival | 15–30 real seconds; installed vanilla rift candidate hook plus additional ground/clearance checks; portal closes after setup |
| No arrival position | Skip that storm's event safely and log; never silently relocate into inaccessible terrain |
| Absence | Shared 30-block / 900-real-second clock; returning to a living loaded chanter resets it; unloading does not pause the clock |
| Abandonment | Release/end the prolonged storm and remove tagged encounter entities; no victory payout |
| Guards after victory | Existing guards remain; no new reinforcement, material harvest or stability-recovery rewards |
| World shutdown | Remaining encounter clocks persist and pause during shutdown |
| Control timing | Commit at heavy-storm start; toggles affect future storms, with no late arrivals or active cancellation |
| Control authority | One placed authority, owner/admin operation; removal restores allowed; recovered generations revoke obsolete copies |

## Rewards and progression

Eligible player-caused chanter deaths provide the V5 ordinary supply rows and independent 25% extra-gear roll, plus the guaranteed progression fragment defined in the entity asset. A full clear with four eligible player kills grants four extra rusty gears, one temporal gear and two ordinary clean bandages. The one opening bell provides its finite salvage once. Environmental/admin kills do not grant the scripted player rewards. This is the alpha's explicitly smaller reward implementation.

**Not implemented:** the four 15% signature equipment effects, Rescue dressing, Listener's lamp, manuscript placement, new special defender variants, Relief Engine/Empty Stretcher, full regional ruin collection, or the proposed lane/recall boss moveset. The craftable Relay Lamp is a separate V11 field furnishing; ordinary bandages retain their vanilla behavior. The original proposals remain in [LOOT_DESIGN.md](LOOT_DESIGN.md), [DLC_CONCEPT.md](DLC_CONCEPT.md) and [ENDGAME_DESIGN.md](ENDGAME_DESIGN.md).

The current Custodian has one 420-HP pool, stock chase/melee (9 base blunt damage, tier 2), health-based acts and a delayed visible radial pulse. It is a simpler guardian than the full proposed endgame boss. Its retreat leash resets the fight rather than yielding a victory. Defeat persists before delivery of the guaranteed controller. Sneak-use the Route Chart at the cleared Far Relay to recover a lost controller without another kill.

Station positions are tied to seed and original spawn, with default horizontal bands 400–650 / 1,600–2,400 / 6,000–8,000 blocks. Small maps clamp positions in bounds. Actual structures are compact open ruins rather than the complete narrative galleries in the proposal. Generation validates loaded natural terrain, fluids and claims before writing, avoids overwriting existing blocks, and leaves unsuitable sites pending. Route messages show their actual saved destinations. P7–P8 remain Archives-only and are not distributed by this build.

`assets/procession/config/progression.json` is a documentary content catalog, **not a live balance configuration**. Loaded server config `ModConfig/procession.json` exposes encounter chance and arrival timing. Other defaults currently live in the core and adapter source.

## Build and evidence

Run `pwsh -NoProfile -File tools/Build-Mod.ps1 -GamePath 'E:/Games/Vintagestory'`. This runs the framework-free behavioral checks and static asset validation, compiles against local game assemblies, and creates `dist/procession-0.1.0.zip` plus its file manifest. It does not install into the user's game. No upstream game DLLs or game textures are bundled.

The installed game is Vintage Story 1.22.7 running .NET 10.0.7. The adapter project uses the installed SDK 9 compiler and explicit .NET 10 runtime references because a .NET 10 SDK is not installed. The core targets .NET 8. `Build-Mod.ps1` discovers the local .NET 10 reference directory; override `-RuntimeReferencePath` if needed. This is a local Windows build workflow, not a claimed portable CI environment.

Evidence levels are separate: behavioral tests exercise the game-independent coordinator/control model; static checks validate assets and references; a successful compilation validates API bindings; isolated server loading validates registration/startup. None proves client rendering, combat fairness, full in-game save/reload behavior or multiplayer interaction correctness. See current verification results in [CURRENT_CONTEXT.md](CURRENT_CONTEXT.md).
