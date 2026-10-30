# In-game verification for development alpha 0.1.0

Use a disposable test profile/world. Both clients and server require the mod. Keep the original worlds and ordinary Mods folder separate from this development test. The local build script creates a ZIP; it does not install it for you.

Build: `pwsh -NoProfile -File tools/Build-Mod.ps1`. Public investigation: `/procession guide`, or use a Route Chart. Craft a chart from cloth, charcoal and a stick, or use the cheaper guaranteed-fragment recipe. `/procession` reports the event state. Admin `/procession test` starts a real test storm and encounter; use it only in the disposable world. `/procession survey` retries safe pending-site surveys; it never authorizes overwriting construction. Server-console `processionaudit` checks asset and installed-API registrations.

## Storm encounter

- Verify light/medium storms never create the encounter and one heavy storm creates at most one. Chance zero must suppress it. Configured ordinary temporal storms remain functional after the encounter.
- Check the directional 15–30-second cue, accessible arrival site, portal closure, four identifiable passive figures and opening family roster plus one bell. Attempt blocked, steep, water and underground placement.
- Compare equal melee and projectile hits on unarmored chanters; confirm 40 HP and 50% ranged resistance. Damage two, kill one, and verify all surviving figures fully heal without dead figures reviving. Repeat with a survivor unloaded/reloaded.
- Observe 8 counted guards maximum plus 1 opening bell, including bell-related replenishment. Lure guards away and unload them; those living guards must still occupy slots. Kill the bell and prove it never respawns.
- Kill weak then strong guards and reverse the order. Check nonadditive extension, fresh 3–5-second pacing after the pause, no catch-up burst, and initial empty-slot filling. Kill a specialist and verify no new member of that family appears from any source.
- Compare ordinary storm stability drain at 4/3/2/1 survivors. Confirm geographic instability remains distinct. A single survivor keeps the storm active with normal storm drain; the final death ends it through vanilla cleanup.
- Leave all survivors by more than 30 blocks for 900 real seconds, including chunk unload, then return. Verify no permanent storm lock, delayed stragglers or abandonment rewards. Returning early to a survivor resets absence. Repeat with everyone dead/offline.
- Save/restart during arrival, pause, refill, partial deaths and abandonment. Confirm dead IDs, healing obligations, cap ownership, last-storm decision and remaining clocks persist. Check no orphaned portal or duplicate arrival.
- Try harvesting, ripping, hacking, catching and trapping event guards before/after victory and after restart. They must provide no harvest, direct material drops, conversions or stability recovery. Independently spawned vanilla mobs must keep ordinary behavior.
- Confirm each player-caused chanter source pays once, completion requires all four eligible player kills, and the one-time bell pays once. Environmental kills, repeated callbacks and reload must not duplicate payouts. V5 signature items are not in this build.

## Exploration, boss and world control

- With zero storm kills, read a peaceful chart and reach all three saved destinations. Repeat mapless. Inspect station placement beside claims, vegetation, water, uneven ground and player structures; no existing block may be overwritten. Pending surveys must remain retryable and honest.
- On a small map inspect clamping and possible site overlap; do not treat the default distance bands as guaranteed there. Test a previously generated disposable world as well as a new one.
- Engage, retreat, die, disconnect, unload and restart at the Custodian. Confirm one live guardian, persistent identity/health, explicit leash reset, readable pulse, no damage through walls, and no false victory. Force-remove the boss in a test copy and check delayed recovery after the full leash area is loaded.
- Defeat the boss, leave before pickup, reload and return. Verify the unlock and controller are recoverable with a full inventory. Sneak-use the chart at the cleared site to recover a lost Bell; old copies must become inert.
- Place the earned Bell, toggle both directions, remove it, reinstall, unload its chunk, log out its owner and restart the server. Confirm one authority and durable world scope. Unauthenticated/creative/stale copies must not grant control. Only the owner/admin may operate or move the active Bell.
- Toggle immediately before/after heavy-storm start and during arrival/combat. Confirm future-only effect, no late spawns, no deletion of an active event, and ordinary storms continuing.
- Check client mesh/texture/animation loading, headroom/hitboxes, ordinary-material crafting, guidepost orientation, light behavior, controller feedback and both-player synchronization. The mesh preview is not an in-game screenshot.

Record game version, mod list, save/restart boundary and relevant server/client logs for failures. Headless loading and core unit checks do not substitute for these interactions.
