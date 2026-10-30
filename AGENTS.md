# Working agreement

Read `docs/CURRENT_CONTEXT.md`, then `docs/DESIGN.md` and `docs/OPEN_QUESTIONS.md` before meaningful work. Consult `docs/RESEARCH.md` and `docs/VISUALS.md` when relevant.

## Scope and truth

- Current phase: development alpha. On 2026-10-29 the user explicitly authorized a project overseer using subagents to expand content/progression and create the actual mod. This supersedes the earlier design-only scope. Read `docs/IMPLEMENTATION.md` for delegated playtest defaults and implementation limits.
- Current explicit user instructions outrank documentation. Once code exists, distinguish tested/persisted behavior from approved design; report and resolve discrepancies rather than silently changing the design.
- `docs/DESIGN.md` records agreed mechanics. Proposed formulas, assignments, and open edge cases are labeled as such. Do not promote them to requirements without user direction.
- Preserve unrelated work. Do not install mods, alter game files/worlds, publish, or invent finished functionality as part of documentation maintenance.
- Public source publication authorized October 30, 2026; no packaged public release has been created. Local development packages are built under `dist/`; no mod has been installed into the user's normal game profile.

## Design invariants

- Heavy storms only; current encounter chance is 100% while the event is allowed. The user selected an earned world-wide event-control reward; ordinary temporal storms continue. Exact control timing/operation remains proposed.
- Four passive, visually distinct chanters; about 40 HP each and 50% ranged damage resistance.
- A chanter death fully heals all surviving chanters, never revives dead ones.
- Eight living defenders share one allowance; the single opening bell is additional and never replaced.
- Bell reinforcements also consume the shared allowance and obey the shared pause/pacing.
- Defender kills buy a health/strength-based shared reinforcement pause. A new kill can extend, never shorten, the remaining pause. Delays do not add.
- Last living chanter sustains the storm with normal storm stability drain. Final death ends the storm.
- These are noncombatant storm sustainers, not a summoning ritual for another entity.

## Updates and validation

Update the canonical specification, open questions, and compact context together when decisions change. Keep research provenance and provisional values explicit. Check relative Markdown/image links and review for contradictory numbers. Build with `pwsh -NoProfile -File tools/Build-Mod.ps1`; it runs core behavioral tests and static asset checks before compiling/packaging. Do not report gameplay verification from those checks. `docs/PLAYTEST.md` covers the remaining runtime gates: save/reload, despawn recovery, cap enforcement, shared healing, storm termination, client rendering, and multiplayer scope.
