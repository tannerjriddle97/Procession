# The Procession — encounter design

**Status:** agreed encounter specification with a development alpha now in source. The user's 2026-10-29 instruction authorizes subagent-led content/progression development and creation of the mod. [IMPLEMENTATION.md](IMPLEMENTATION.md) records what the alpha implements, the delegated defaults resolving previously open details, and deferred expansion features. [CURRENT_CONTEXT.md](CURRENT_CONTEXT.md) records verification. Values remain playtest targets rather than measured balance.

The historical proposal discussions below are retained as design provenance. Their earlier documentation-only restrictions do not override the latest development authorization. Delegated implementation choices are not recast as prior user approvals.

## DLC expansion proposal

On 2026-10-29 the user requested a full imaginative enhancement pass, including more creatures, items, and content, while deferring code and functionality. [The Procession: Until They Answer](DLC_CONCEPT.md) develops that request into a DLC-scale proposal: eight additional defender variants within the existing families, three separate ruin enemies including the later-requested final boss, six regional ruin types plus a distant final site, twelve new practical items alongside the six existing loot proposals and a separate control reward, cosmetic/building content, and a connected optional investigation.

This authorizes expansion concept work, not blanket approval of its precise mechanics. Newly introduced DLC content, including a proposed limit of two special-behavior defenders within the existing eight slots, remains proposed except for the subsequent endgame direction recorded below. V5 loot remains provisional, P7–P8 stay Archives-only, and no implementation or playable content is claimed.

Subsequent user-requested [DLC mob concept sheets](VISUALS.md) visualize the Relief Engine, Empty Stretcher, and four defenders. These are visual proposals, not final models or additional approved mechanics.

## Earned event control — accepted direction

On 2026-10-29 the user requested a final boss at a distant temporal ruin and an earned item/object allowing the player to choose whether the Procession occurs. In follow-up, the user explicitly selected **world-wide control**, with ordinary temporal storms continuing. The baseline heavy-storm trigger is therefore conditional on the event being allowed; it is no longer unconditional after obtaining and using that control.

[The Far Relay endgame proposal](ENDGAME_DESIGN.md) recommends a separate final destination, the Last Custodian boss, and a placeable Closing Bell with allowed/suppressed modes. Names, geography, combat, guaranteed reward delivery, free switching, next-storm-only timing, placement/removal rules, persistence details, and multiplayer permissions remain proposals. In particular, world-wide control is accepted; using it to cancel an already active encounter has not been approved.

The final boss is a separate expedition, not a being summoned by chanter deaths. When a procession occurs, all four chanters and their approved combat, cap, healing, pause/pacing, and storm-termination rules below remain unchanged.

## Goal and trigger

Make heavy temporal storms scary and interactive by adding vulnerable, passive storm sustainers behind dangerous defenders. The player can venture out to end that storm through combat, accept a long storm while leaving the procession to despawn, or eventually earn control over whether later Processions occur.

- While the event is allowed, spawn one procession of four chanters during **every heavy storm** (100% baseline chance for initial playtesting).
- Never spawn a procession in light or medium storms.
- Event chance should be configurable for later tuning.
- Earned world-wide control allows the player to suppress the Procession while ordinary temporal storms continue. Exact control timing/operation remains under review in the endgame proposal.
- No additional entity is summoned as a ritual finale. The event directly sustains the storm and accelerates stability loss.

## Arrival

1. A **large rift/portal** appears at the start of the heavy storm.
2. A positional hum/buzz builds for approximately **15–30 seconds**, providing a directional audio cue through low visibility.
3. All four chanters walk out and take their positions.
4. Setup spawns **one shiver, one bowtorn, one locust, one drifter, and one bell**. The bell provides the audible signal that the encounter has begun.
5. The procession's extra stability drain and storm lock activate when setup completes.

Use the game's existing rift placement rules as the intended placement baseline. Actual rule details, clearance, accessibility, and a failed-placement fallback need research before implementation. Portal persistence/closure has not been chosen.

## Chanters

- Four nonattacking chanters with unique visuals and identifiable silhouettes.
- Approximately **40 HP each**.
- **50% ranged damage resistance**; melee remains the intended way to finish them efficiently.
- Whenever one dies, **all surviving chanters heal to full**. Dead chanters remain dead.
- Killing the final chanter ends the temporal storm immediately and removes the procession's extra stability drain.

### Stability progression

Multipliers apply to the **ordinary temporal storm drain**, not total stability or damage.

| Living chanters | Drain multiplier | Storm lock |
|---|---:|---|
| 4 | 3x | Active |
| 3 | 2x | Active |
| 2 | 1.5x | Active |
| 1 | 1x — ordinary storm drain | Active |
| 0 | Extra drain removed; storm ends | Released |

Geographic/player scope of the multiplier has not been settled.

## Defender families

| Specialist | Own family | Shared family |
|---|---|---|
| Shiver chanter | Shivers | Drifters |
| Bowtorn chanter | Bowtorns | Drifters |
| Locust chanter | Locusts | Drifters |
| Bell chanter | One opening bell, never replaced | Drifters |

- **Any tier** is eligible in its family. Earlier gearfoot-minimum proposals are superseded.
- Exact entity lists, variants, selection weights, and specialty shutdown accounting remain to specify.
- Intended target-selection benefit: killing a specialist stops new spawns of its family. Earlier discussion proposed this; the cross-source rule and bell child pool still need explicit confirmation.

## Cap and opening roster

- Maximum **eight living procession defenders**, shared across all chanters and reinforcements from their bell.
- The **opening bell does not consume a defender slot**: at most eight counted defenders plus one bell, excluding the four chanters.
- Only **one bell may spawn for the entire procession**. It never respawns after death.
- The initial five-mob roster occupies **four of the eight counted slots**, plus the separate bell.
- Ordinary vanilla storm monsters are outside this procession cap. The total encounter population can exceed nine enemies.
- A living defender cannot free a slot merely by being lured away; exact loaded/unloaded tracking needs planning.

## Shared reinforcement pause

- A defender death starts a **shared pause across all procession reinforcement sources**, including the bell.
- Its duration depends on the killed creature's maximum HP/strength. Weak drifters should buy about **five seconds**, stronger enemies such as gearfoot bowtorns roughly **15–20 seconds**.
- Another kill sets remaining pause to `max(current remaining pause, new kill delay)`.
- Delays **do not add together**. A weak kill never shortens an existing longer pause.
- There is no independently cycling replacement timer that can spawn a creature immediately after a kill.
- When the shared pause expires, use a fresh random **3–5-second delay before every replacement**, including the first, until the cap is reached or a new kill pauses spawning.
- Do not queue missed intervals or release a catch-up wave. Spawn at most one replacement per pacing interval.
- Replacements use the currently available family pools. Bell children obey the same cap, pause, and pace, rather than independent vanilla timing.

Example: a gearfoot kill grants 18 seconds. Two seconds later, a weak drifter kill worth five seconds leaves the existing 16-second pause intact. Once that pause expires, the first reinforcement waits another 3–5 seconds.

### Provisional timing formula — not finalized

Suggested in discussion: `min(20, 5 + 0.3 * max(0, maxHP - 12))` seconds, plus possible explicit threat bonuses (e.g. +2 for gearfoot) and around +/-10% randomness. The shared-pause principle and approximate weak/strong durations were accepted; **the equation, bonuses, jitter, and whether the hard cap applies before/after modifiers were not**. Do not implement this as settled config.

The original fixed random 8–15-second replacement delay and separate per-slot countdowns are superseded. The accepted 3–5-second refill cadence is separate from the kill-based shared pause.

## Despawn and storm recovery

- Use bell-like absence rules: **30-block player-distance threshold**, with **15 minutes out of range** before despawn.
- Leaving instead of fighting therefore means enduring the extended storm while the procession's absence timer runs.
- Once the procession is gone, its storm lock must be removed; it must not strand the world in a permanent storm.
- Exact group-versus-individual timer rules, countdown reset on return, chunk-unloaded handling, and immediate storm termination versus normal expiry processing need specification.
- Save/reload and player death handling are not yet designed.

## Visual direction

Four distinct blocky forms, restrained weathered materials, and subtle teal emissions readable through darkness/fog. Use the [generated references](VISUALS.md) as visual guidance, not final game-ready assets.

- The Veiled: tall hood, black facial recess, narrow mouth grille.
- The Resonant: cage-mask head and rusted chest plates.
- The Bound: hunched, linen/binding-wrapped figure with elongated arms.
- The Hollow / bell specialist: revise the reference's closed rectangular head to an **open U-shaped tuning fork**, with parallel cuboid prongs joined at the base. Faint inner-edge glow; subtle vibration during chanting is a suggested animation, not finalized.

## Lore direction

On 2026-10-25 the user accepted the **lost rescue operation** direction: an abandoned apparatus used to maintain contact with people beyond a dangerous passage returns in four otherworldly forms during heavy storms. Their coordinated activity continues a rescue call and sustains the temporal connection; dangerous creatures answer. This is original mod lore grounded in existing accounts of bell/recording technology, not a claim about canonical creature origins or the cause of all temporal storms.

The four are independent of the main lore characters. Whether they contain former operators, altered equipment, or something else remains uncertain. No divine motive, summoned boss, or ritual finale is part of the selected direction. Broad roles are caller, tender of mechanical helpers, holder/brace, and listener; final names and the first three specialty-to-appearance assignments remain open.

See [direction details and historical alternatives](LORE_DIRECTIONS.md) and [eight original collectible drafts](LORE_COLLECTIBLES.md). The user requested two additional records from the quartet's viewpoint showing entrapment and gradual mental deterioration, found **only in the Resonance Archives** (P7–P8). Their testimony reveals subjective experience without settling their exact original nature. Draft prose, original supporting characters, manuscript formats/positions, and incidental observations require review before becoming final design. They do not authorize additional mechanics.

## Deferred

Lore evidence is recorded in [LORE_RESEARCH.md](LORE_RESEARCH.md); [coverage](LORE_SOURCE_COVERAGE.md) distinguishes reviewed and pending sources. Direction A in [LORE_DIRECTIONS.md](LORE_DIRECTIONS.md) has been selected; B and C are historical alternatives. Eight collectible drafts exist for review. P7–P8 are Archives-only by user instruction; precise acquisition, wording, and remaining placements are not finalized.

Collectible review and loot remain open. Location/rift rules, exact roster weights, timing formula, portal lifecycle, remaining despawn/persistence rules, multiplayer scope, balance and drops are unresolved. No code or installable mod is authorized by this design record alone.

Loot direction update, 2026-10-25: the user requested meaningful player power to reward the increased difficulty of heavy storms. [LOOT_DESIGN.md](LOOT_DESIGN.md) V4 proposes three equipment relics alongside the vanilla supply foundation; [LOOT_REVIEW.md](LOOT_REVIEW.md) records the new review. This approves the direction of proposal work, not those exact effects, numbers, probabilities, or delivery rules. Approved encounter values above are unchanged; any future personal drain resistance is an explicit proposed item effect, not a reduction of the encounter's emitted multiplier.

Further loot direction, 2026-10-26: the user requested one powerful themed item per “miniboss” at about15% rarity. [LOOT_DESIGN.md](LOOT_DESIGN.md) V5 supersedes the V4 pool with one signature per chanter visual, independently rolled on eligible death. Exact items/values remain proposed; “miniboss” does not change the four's approved passive nature. All encounter numbers and lore identities above remain unchanged.
