# Current context

## Current state

- Date: 2026-10-29, America/New_York. Project: `<project-root>`.
- **Phase: actual development alpha.** The latest user instruction authorizes an overseer using subagents to expand content/progression and create the mod, superseding the earlier documentation-only phase. Three subagents developed progression/sites, runtime integration and original assets; independent reviews corrected significant defects.
- Working title The Procession; mod ID `procession`, development version `0.1.0`. Public source publication authorized October 30, 2026; no packaged public release or selected open-source license. No installation into the normal game profile or changes to existing worlds. A disposable server uses only workspace `artifacts/`, bound to localhost.
- Actual source/build: `src/Procession.Core`, `src/Procession.Game`, `tests/Procession.Core.Tests`, `assets/procession`, `tools/Build-Mod.ps1`. Output: `dist/procession-0.1.0.zip` plus SHA256 manifest. Regenerate after runtime changes.
- [IMPLEMENTATION.md](IMPLEMENTATION.md) records actual scope/defaults; [DESIGN.md](DESIGN.md) retains agreed mechanics; [PROGRESSION.md](PROGRESSION.md) develops the playable route. Prior context/history remains in [CONTEXT_BEFORE_IMPLEMENTATION.md](CONTEXT_BEFORE_IMPLEMENTATION.md).

## Implemented alpha

One shared heavy-storm Procession, four original passive chanters, coordinated vanilla-family guards and an additional one-time bell, finite ordinary rewards, persisted ownership/clocks, storm lifetime/drain integration, three saved expedition landmarks, a separate 420-HP melee/pulse Custodian, and an earned placed Closing Bell with future-only world-wide control. Route Chart, Relay Fragment, Guide-Post, Four-Plate Memorial and Relay Lamp add exploration/building content. Ordinary-material chart crafting permits zero-storm-kill exploration.

Invariants remain: 40 HP each, 50% ranged resistance, every death fully heals living survivors; 8 shared guards plus one bell; nonadditive kill pause, fresh 3-5-second cadence; 3/2/1.5/1 ordinary storm-drain multipliers; final death ends the storm. The ruin boss is separate. Ordinary temporal storms continue when Processions are suppressed.

Delegated playtest defaults: Bound/shiver, Veiled/bowtorn, Resonant/locust, Hollow/bell; group-wide 30-block/900-real-second absence; global encounter drain; specialist family shutdown; initial slots fill at normal cadence; bell death gives capped pause; availability latched per heavy storm; one controller generation/authority, owner/admin operation, removal restores allowed, recovery at the cleared Far Relay. These are autonomous development choices, not retroactive user approvals. See [IMPLEMENTATION.md](IMPLEMENTATION.md).

The full DLC remains a backlog: V5 signature effects/15% drops, bespoke dressing/lamp, manuscripts, special defenders, Relief Engine/Empty Stretcher, all regional ruins and lane/recall boss. P7-P8 remain Archives-only and no records are distributed yet. Lost rescue-operation lore remains ambiguous, with no main-character identities or confirmed creature origins.

## Verification

- **25 core behavioral checks pass:** cap/identity accounting, pause/cadence, initial filling, staged drain/lock, family shutdown, JSON reload, absence recovery, future-only decisions, permissions and controller generations. Three quantitative drain cases prevent multiplying lost positive recovery as drain.
- **29 JSON files; 2,157 static asset checks pass.** Shapes/textures, animation targets, names, chanter passivity/health, bell coordination and references resolve. Seven original PNG materials; no upstream textures bundled. [Mesh preview](../tools/content/mesh-preview.png) is geometry evidence, not a game screenshot.
- Adapter compiles against installed **Vintage Story 1.22.7: 0 warnings, 0 errors**. Game runtime .NET 10.0.7; compiler SDK 9.0.302 with explicit runtime references; core .NET 8. No SDK installed.
- Corrected-package **fresh-world startup, clean shutdown and same-world restart pass** on an isolated localhost server. Both audits confirm six entity types, two items, Closing Bell, four runtime hooks and world initialization. Both final logs contain zero errors/exceptions. This tests empty-server initialization/persistence, not live entity encounters or controller interactions. See [VERIFICATION.md](VERIFICATION.md).
- The first smoke run exposed spawn not being ready during SaveGameLoaded; planning now defers/retries. Reviews also corrected drain math, guard behavior preservation during clone, harvest leakage, controller generations/recovery and portal persistence.
- **Interactive gameplay unverified:** rendering/animation, actual combat/drain, entity save/reload/unload, caps/healing/storm exit, controller interactions and multiplayer. Follow [PLAYTEST.md](PLAYTEST.md). Pure tests, compilation and startup do not prove these.

## Continuation

Test the live adapter in a disposable client world next. Prioritize confirmed crashes/invariant failures before expanding content. Follow [OPEN_QUESTIONS.md](OPEN_QUESTIONS.md); do not announce the full expansion complete or install into existing worlds without authorization.

Research remains installed 1.22.7 assets plus separately inspected public source. This pass also inspected exact installed managed metadata/IL for storm/rift hooks, health, behavior cloning and temporal-drain interpretation. Base-game-only compatibility is the current target; other stability-hook subscribers and transformations need testing.
