# Verification

`dotnet run --project tests/Procession.Core.Tests` runs the framework-free behavioral suite. `pwsh -NoProfile -File tools/Test-Assets.ps1` checks original asset references/structure. `tools/Build-Mod.ps1` runs both before compiling/packaging.

These do not verify engine interactions. Follow [PLAYTEST.md](../docs/PLAYTEST.md) for actual save/reload, caps, healing, storm exit, progression, control, rendering and multiplayer.
