# The Procession

A development alpha for **Vintage Story 1.22.7**. Heavy storms bring four passive chanters protected by a shared defender force. Silence the storm, investigate three rescue stations, defeat a distant guardian, and bring home world-wide control over future Processions. Ordinary temporal storms continue.

The project contains actual C# gameplay code, original cuboid assets, recipes, progression and a local mod ZIP. Interactive combat/rendering/balance checks remain. See [implementation and limits](docs/IMPLEMENTATION.md), [current evidence](docs/CURRENT_CONTEXT.md) and [playtest instructions](docs/PLAYTEST.md).

## Build and try

```powershell
pwsh -NoProfile -File tools/Build-Mod.ps1 -GamePath 'E:/Games/Vintagestory'
```

The script runs core behavioral tests and asset checks, compiles against the installed game and creates `dist/procession-0.1.0.zip` with a per-file SHA256 manifest. It uses the installed SDK 9 compiler plus explicit .NET 10 runtime references. No NuGet packages or game binaries are downloaded/bundled. This is a local Windows build workflow.

Use the ZIP in a **separate test profile**; server and clients both require it. The build does not install it or edit existing worlds. `/procession guide` supplies mapless directions. A reusable Route Chart is craftable from ordinary supplies or a guaranteed chanter fragment. Sneak-use a chart at the cleared Far Relay to recover a lost Closing Bell.

## Included

- Four original passive 40-HP chanters, ranged resistance, survivor healing and last-death storm termination.
- Eight shared guard slots, one additional opening bell, shared kill pause and paced replacements.
- Relief Shelter, Lower Relay and Far Relay: saved destinations, safe terrain surveys and no rare-drop entrance gates.
- A 420-HP Last Custodian using melee and a delayed radial pulse; persistent owner/admin-operated Closing Bell.
- Finite ordinary rewards, Route Chart, Relay Fragment, craftable Guide-Post, Four-Plate Memorial and Relay Lamp.

V5 signature effects, bespoke dressing/lamp, manuscripts, special defenders, Relief Engine/Empty Stretcher and the full lane/recall boss remain future content. The broader [DLC proposal](docs/DLC_CONCEPT.md) stays a backlog.

## Continue development

Read [CURRENT_CONTEXT](docs/CURRENT_CONTEXT.md), [DESIGN](docs/DESIGN.md), [OPEN_QUESTIONS](docs/OPEN_QUESTIONS.md), [IMPLEMENTATION](docs/IMPLEMENTATION.md) and [PROGRESSION](docs/PROGRESSION.md). [AGENTS.md](AGENTS.md) governs continuation. [Visuals](docs/VISUALS.md) distinguish concepts from original meshes and game evidence.

`src/Procession.Core` holds engine-independent coordination/control; `src/Procession.Game` is the adapter; `assets/procession` contains runtime content; `tests` holds behavioral checks; `tools` builds, validates and regenerates content. Public source repository: https://github.com/tannerjriddle97/Procession. No packaged public release or open-source license has been selected.

## Source repository

[Procession](https://github.com/tannerjriddle97/Procession) contains the addon source, authoring files and available test sources. No open-source license has been selected for this source. Game/mod dependencies, local profiles, saves, recovery provenance and generated packages are excluded. Historical audit/probe scripts require a separate deliberate runtime-testing decision; publication does not run them. Interactive acceptance limits remain as documented.
