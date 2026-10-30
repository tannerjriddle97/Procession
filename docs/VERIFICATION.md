# Verification record — 2026-10-29

This records actual checks for development alpha 0.1.0. It does not claim the full DLC, balanced combat or verified multiplayer.

## Automated and compiled

- `dotnet run --project tests/Procession.Core.Tests --configuration Release`: **25 behavioral checks pass**. These exercise the engine-independent model, JSON restoration, controller permissions/generations and quantitative storm-drain math. They do not instantiate a game world or prove adapter behavior.
- `tools/Test-Assets.ps1`: **29 JSON files, 2,157 static checks pass**. References, shape elements/animation targets, localization, chanter health/passivity and bell coordinator boundaries validate. This does not render the client.
- `tools/Build-Mod.ps1`: adapter compiles against exact installed Vintage Story **1.22.7** assemblies, **0 warnings, 0 errors**. Original assets and two mod DLLs package into `dist/procession-0.1.0.zip`; the adjacent manifest records file hashes. No upstream assemblies/textures are included.
- **139 relative Markdown/image links resolve**, including the preserved pre-implementation context. Absolute research references were excluded from that relative-link check.

## Isolated server smoke

The actual game server ran from the installed distribution with a workspace-only data directory, the local ZIP as an additional mod path, `--ip 127.0.0.1 --port 42438`, and server advertising/UPnP disabled. No normal Mods directory, existing world or user server configuration was edited.

The initial test caught a real fresh-world initialization error: `DefaultSpawnPosition` was unavailable during `SaveGameLoaded`. Site planning now defers until normal ticking and safely retries. A deliberately small-map test also produced vanilla out-of-bounds world-generation warnings; final smoke used default map dimensions instead. Small-map gameplay remains a separate test gate.

Final default-size fresh run at approximately 16:22 America/New_York:

```text
Dedicated Server now running on Port 42438 and ip 127.0.0.1!
Procession audit passed: 6 entity types, 2 items, Closing Bell and 4 installed-runtime hooks present.
World loaded: True. This validates registration, not combat or rendering.
All threads gracefully shut down
```

The same disposable world was restarted with time passing while empty; the same audit passed at 16:23:52, followed by a clean shutdown at 16:24:02. Both final `server-main.log` snapshots contain **zero errors/exceptions**. Evidence remains locally at `artifacts/smoke-fresh.log` and `artifacts/smoke-reload.log`; the test world is under `artifacts/smoke-default`. All test server processes were stopped.

The engine's remote blocked-mod-list lookup was unavailable under restricted network access; local loading proceeded. The console audit inspects registered assets and installed private/public hooks. It deliberately does not initialize temporary AI entities under a read-only command.

## Remaining evidence gaps

No client joined. No live encounter, ruin, guardian battle, physical controller, projectile hit, actual chanter heal, guardian recovery or multiplayer permission interaction was exercised. The restart establishes empty-server initialization/world-data restoration only. Follow [PLAYTEST.md](PLAYTEST.md) for those game interactions and client rendering. The original mesh preview is a geometry visualization, not a game screenshot.
