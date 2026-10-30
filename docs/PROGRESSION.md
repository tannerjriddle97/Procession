# The Procession — playable progression

**Development specification, 2026-10-29.** The user now authorizes an overseer and subagents to expand the content/progression and create the mod. This supersedes the earlier documentation-only scope. Choices marked **playtest default** below are delegated development decisions, not claims of earlier user approval or measured balance. Actual implementation and verification belong in the release notes and current context.

The expansion's playable spine is **notice the returning call → prepare and investigate → find the Lower Relay → travel to the Far Relay → defeat its custodian → choose whether future Processions occur**. Storm combat and exploration are parallel activities. Neither reading every note nor repeatedly farming storms is an entrance requirement.

## What the first version needs

The first version concentrates on four visually distinct passive chanters, the shared defender encounter, a reliable expedition route, a separate final guardian, and earned world-wide control. A small number of recognizable places is preferable to seven largely empty locations. The broader eleven-enemy, seven-site expansion remains a content backlog; a catalog entry or concept image does not make it playable.

**Implemented source, pending gameplay verification:** `ProgressionSites` saves the three site positions, builds small masonry/timber landmarks in validated loaded terrain, announces first-visit directions, provides a finite shelter chart, and spawns one saved Custodian at the Far Relay. `/procession guide` supplies a reusable mapless route without needing an item. If terrain around a destination is unsuitable, `/procession survey` validates a visitor's nearby open patch within 512 blocks and replans that same pending site without overwriting blocks. Low natural vegetation is retained, including gaps where a planned ruined wall would intersect it. The catalog below is documentary; runtime does not load its tuning fields.

The implemented Custodian has 420 HP, a 9-damage stock melee attack, and custom warned radial pulses with health-based phases. Pulses have a 1.8-second visible buildup, radii 5/6/7 blocks, and pre-armor damage 5/7/9 for the three thirds of its health bar; their cooldown is 10/10/7 seconds. This is an initial alternate moveset, not the proposed remembered strike lanes. The guardian returns home and heals if pulled beyond its 48-block leash. These are autonomous source defaults with no claim of tested difficulty.

Defeat saves the world unlock before issuing the Bell. A player can recover a lost Bell by **sneaking while using the route chart at the cleared Far Relay**. Recovery advances the saved generation, revokes an old installation, and invalidates old item tokens; when a Bell is installed, its owner or an administrator must perform that replacement. Missing-boss recovery reconciles its identity and saved health only after the entire larger leash volume has remained loaded for 30 seconds. These safeguards are implemented source paths; save ordering, load timing and interrupted delivery still need engine tests.

The initial chart/fragment, three mundane building pieces, Closing Bell and six entity assets do not implement the four V5 signature relics, bespoke dressing/lamp, manuscript placement, regional miniboss wing or eight new defenders. First-visit chat currently stands in for an interactive route board. Landmark geometry is compact and open rather than the seven-room concept layouts.

| Chapter | Available activity | Persistent result | Required to continue? |
|---|---|---|---|
| First heavy storm | Follow the sound, fight individual chanters or retreat | Finite source rewards; knowledge of silhouettes, healing and the last survivor | No; ruins can be explored first |
| Relief Shelter | Find the route equipment and survey the next station | Written bearing/distance to the Lower Relay | No; either relay can be found organically |
| Lower Relay | Inspect the four deserted workstations and a route board | Directions to the distant Far Relay; P5/P6 contextual home in a later manuscript pass | No key or miniboss kill required |
| Far Relay | Prepare outside a conspicuous receiving court; approach its custodian | Defeating the one guardian permanently earns control for the world | Boss victory earns control; signatures and manuscripts do not |
| Homecoming | Place and operate the Closing Bell | Reversible allowed/suppressed setting for future heavy storms | Optional; ordinary temporal storms continue |

No XP, skill tree, faction standing, rare map roll, four-signature set, or Archives visit gates this route. A route chart is an optional convenience. A guaranteed fragment from any chanter can provide a second, combat-based chart route; it must not become a mandatory multi-storm collection grind. No key is consumed to enter the final site or damage its boss.

## Places that lead somewhere

**Playtest defaults:** one compact Relief Shelter, one Lower Relay and one Far Relay per world, with positions tied to the original world spawn and world seed. Distances are measured horizontally from that spawn, never the player's latest home. Initial placement bands are 400–650, 1,600–2,400 and 6,000–8,000 blocks respectively. These are design targets, not measured travel times. Small maps need in-bounds clamping and explicit disclosure if they cannot support a distant expedition.

The shelter is a broken roof and low weather wall around a work marker. The Lower Relay combines an open desk court and four inert stations, with a clear way out. The Far Relay has a recognizable ruined outer wall, broad entry, preparation space and open receiving court. Initial geometry can be simple cuboid masonry and timber; concept illustrations are not production models.

Each route description gives the destination name, horizontal bearing, approximate remaining distance and coordinates relative to the world spawn. It works without a map mod. The first shelter points onward; the Lower Relay points to the Far Relay. The initial chart lists all three saved sites, so the first clue cannot strand a player. Reading a route never consumes the chart or rerolls the destination. First-visit proximity text currently supplies the survey board's information.

Safe placement has priority over a chosen coordinate. Validate loaded terrain, headroom and the entire footprint before making a structure; reject water, substantial slopes, claims and occupied spaces. Never excavate through player construction or overwrite a settlement to preserve a distance target. Relocation updates the same saved site rather than creating another cache. Existing-world support must use the same checks; it is not permission to install this mod into the user's existing world during development. If no candidate is valid, leave the site pending, expose the failure to the operator, and retain an explicit recovery path rather than silently marking it complete.

## The guardian and the quiet chamber

The Last Custodian is a separate ruin guardian, not a consequence of the fourth chanter's death. The receiving court never becomes another storm anchor. Its purpose is legible before commitment: a service frame keeps watch over the isolation equipment. Its body and work do not establish that it is a missing worker, named lore protagonist, or commander of the quartet.

The full moveset in [ENDGAME_DESIGN.md](ENDGAME_DESIGN.md) remains the combat target. A first implementation may use a clearly documented simpler melee guardian while custom telegraphs, remembered strike lanes and three-act animations are developed. It must not claim that a stock melee task implements the proposed boss. Choose health/damage against the actual installed AI definitions; do not infer runtime balance from this document.

The boss uses one health pool. Retreat or disconnect does not count as victory. A living guardian must retain its saved identity across unload/reload. Missing-entity recovery checks that its destination chunk has actually loaded before recreating it, preventing an unloaded boss from being mistaken for a dead one. Any recreation must preserve saved health or explicitly document a reset; it must never create two live guardians. The site records defeat before delivering the world unlock. A defeated guardian does not respawn just because its old chunk loads again.

Control is guaranteed and independent of ordinary boss loot. A world-shared unlock survives dying before item collection. Recovery at the cleared Far Relay restores a lost control object without another boss victory; replacement invalidates the previous controller authority. Ordinary inventory-full handling leaves a recoverable entitlement or clearly visible world item. Persisted world state, item delivery and crash boundaries require runtime tests; an in-memory flag is insufficient proof.

## Operating the reward

**Playtest defaults:** one authoritative installed Closing Bell per world. Placing it starts allowed. Its owner or a server administrator may switch it; other players can read the state. Picking up/removing the active Bell restores allowed for subsequent storm decisions. An unearned creative copy grants no control. A recovered replacement supersedes the previous binding. The operation is free after victory and remains effective with the controller chunk unloaded or owner offline.

Commit availability once at the start of a heavy storm. A later toggle affects the next heavy storm, without deleting current chanters, ending a current storm, generating a late encounter, or awarding victory. Saved availability is world-wide; active-encounter stability scope is a separate rule. Ordinary temporal storms continue in either setting.

Player feedback should say exactly what changed: **“Procession suppressed world-wide. Ordinary temporal storms continue. Applies to the next heavy storm.”** An allowed-mode message should identify that same timing. An unsuccessful operation must explain whether the boss is undefeated, another Bell is installed, or the player lacks authority.

## Rewards and content breadth

The four source-specific V5 signatures remain the equipment direction: 15% per eligible source death, including partial victories. Rare equipment never gates exploration, storm termination or earned control. Guaranteed progression clues supplement those rolls; they do not silently alter the independent signature probabilities. The V5 effects require real item behavior; a renamed ordinary item must be identified as a placeholder until its promised effect exists.

P1–P6 retain their contextual discoveries; original prose is preserved in [LORE_COLLECTIBLES.md](LORE_COLLECTIBLES.md). P7–P8 remain exclusively in the Resonance Archives, with no fragments, paraphrases or machine playback added to the relays. If manuscript placement is not implemented, ship none of those records as loot rather than scattering them into the wrong places.

The next content increments are the optional Relief Engine/Empty Stretcher wing, practical route lights and guide marks, then new defender variants with readable recovery windows. Craftable tools should use ordinary materials and remain useful without signatures. The twelve practical items, four cosmetic pieces, eight special defenders and ten furnishing families in [DLC_CONCEPT.md](DLC_CONCEPT.md) remain separate work until their assets and behaviors exist. No item catalog should be presented as a completed DLC.

## Verification required for the route

- Reach the Far Relay in a fresh world with zero chanter kills, signatures or manuscripts; repeat on an existing-world test copy without overwriting construction.
- Read directions in mapless play; verify they name the saved destination after any rejected placement candidate.
- Confirm an occupied/claimed/wet/steep site is rejected before a single block is written; an invalid site remains recoverable.
- Retreat, die, disconnect, unload and reload during the guardian encounter; verify one live boss and no false victory.
- Kill the guardian, leave before collecting the Bell, reload and recover control. Check inventory-full and lost-item recovery without another boss kill.
- Try duplicate controllers, creative copies, owner/admin/other-player operations, removal, chunk unload and save/reload.
- Toggle around heavy-storm start/setup. Verify one committed decision, no current-encounter cancellation, no late arrival and ordinary temporal storms continuing.

The machine-readable [progression catalog](../assets/procession/config/progression.json) is a documentary development catalog, not a runtime settings file. Its `implementationStatus` fields distinguish source support from gameplay verification; documentation/data checks do not demonstrate gameplay. Runtime event settings live in the mod-generated `procession.json`; site/control source defaults live in `ProgressionSites.cs`.
