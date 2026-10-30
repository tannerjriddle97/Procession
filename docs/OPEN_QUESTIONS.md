# Open decisions

The user now authorizes development. [IMPLEMENTATION.md](IMPLEMENTATION.md) records delegated playtest defaults and actual alpha scope; those choices resolve implementation blockers without asserting prior user approval. Keep agreed mechanics in [DESIGN.md](DESIGN.md) and verify actual behavior with [PLAYTEST.md](PLAYTEST.md). The older questions below are retained for design/balance review; they do not forbid the authorized work.

## Active alpha work

- Finish interactive checks for storm lifetime/drain, healing, guard caps, pause/pacing, absence and live save/reload. Core tests alone do not verify the adapter.
- Verify fresh/existing-world station placement, safe pending-site resurvey, mapless directions, missing-boss recovery and the full control reward route. Confirm owner/admin operation and duplicate/recovered controller behavior with multiple clients.
- Inspect client shapes, textures, animation, hitboxes and craftable furnishings. The mesh preview is geometry evidence only.
- Tune the 420-HP melee/pulse Custodian and its telegraph. The full remembered-lane/recall fight remains future work.
- Implement and review the V5 signatures and bespoke dressing/lamp before adding their drops. Current finite ordinary supply rewards are documented separately; no signature bonus is represented by a renamed vanilla item.
- Select expansion increments: optional Relief Engine/Empty Stretcher, special defenders, regional ruins, practical tools, manuscripts and furnishing families. P7–P8 stay Archives-only.
- Decide final names, license, compatibility targets and public release process. No Git repository or publication is authorized by a local alpha package alone.

## Current review: final boss and earned world-wide event control

- The user requested a distant temporal ruin, final boss, and an earned item/object to choose whether the Procession occurs. **World-wide control with ordinary temporal storms continuing is selected.** That scope is settled; exact control behavior is not.
- Review [the Far Relay proposal](ENDGAME_DESIGN.md): a separate distant ruin, the Last Custodian, and a placeable Closing Bell. Names, visual form, lore, combat, and reward delivery are recommendations rather than final requirements.
- Choose the site distance from the starting region, reliable discovery route, terrain requirements, and access for existing worlds. Avoid rare-signature or Archives-only prerequisites.
- Review a guaranteed world-shared control reward, two lever states, unlimited switching after earning it, next-heavy-storm-only effect, and returning to allowed on removal. Whether an already active procession can be affected remains unresolved; the proposal recommends preserving it.
- Set installed-controller authority, unloaded-chunk/reload behavior, conflicting replacements, lost-item recovery, multiplayer operation permissions, and saved reward claims.
- Set boss health/damage, three-act thresholds, attack echo limits, retreat/reset behavior, reward/checkpoint safety, and any repeat challenge. The Relief Engine remains an earlier miniboss; the new boss is not summoned at storm completion.

## DLC expansion concept

- Review [Until They Answer](DLC_CONCEPT.md), requested by the user on 2026-10-29 as a design-only enhancement pass. The full package is proposed content, not an approved implementation plan.
- Choose which of the eight new family variants and three separate ruin enemies deserve inclusion, including the additional final boss. Review their tells, recovery windows, status/displacement effects, tier equivalents, and the proposed two-special limit inside the existing eight-defender cap.
- Review three formation layouts, healing/chant feedback, and baseline accessibility. Placement, initial cap filling, bell children, and surviving-guard behavior still need explicit core decisions; the expansion does not settle them.
- Review the six regional ruin archetypes plus the distant Far Relay, Lower Relay layout, optional Relief Engine, finite site rewards, and placement refinements for P1–P6. P7–P8 remain exclusive to the Resonance Archives. New supplementary records R1–R6 are draft pitches and do not replace the original eight.
- Review twelve practical items, four cosmetic pieces, and ten furnishing families. Set ordinary-material recipes, storage/line recovery rules, actual combat values, fuel/duration limits, repair/stacking, and useful everyday tradeoffs before treating them as specifications. V5 signatures and exact loot remain separately unapproved.
- Decide world frequency, regional distribution, existing-world discovery, and finite claim behavior for ruins. A separate ruin boss must not become a summoned finale, new storm anchor, or additional requirement for ending a procession.

## Lore and loot

- Follow the [research roadmap](LORE_RESEARCH.md) and [coverage checklist](LORE_SOURCE_COVERAGE.md): complete relevant primary evidence before proposing identities. Preserve narrator beliefs, inference, canon, and original additions as separate categories.
- The lost rescue operation (Direction A in [LORE_DIRECTIONS.md](LORE_DIRECTIONS.md)) was selected on 2026-10-25. Review the [eight collectible drafts](LORE_COLLECTIBLES.md): wording, distinct voices, invented supporting details, order of discovery, and proposed placements. P7–P8 must be Archives-only, from the quartet's viewpoint, showing entrapment and gradual mental decline. Their exact manuscript format and shelf/acquisition location remain open. Directions B/C remain historical alternatives.
- For the rescue-apparatus proposal: how much human origin should be established, whether the four understand their work, whether bodies predate the accident, and whether the proposed visual/family mapping fits. Its storm/sound connection is original mod lore, not a canon discovery.

- Final creature/event/mod names and exact nature of the four within the chosen rescue premise; how much of their intention the player should be able to infer. The prose drafts do not settle their inner awareness or human ancestry.
- Review [V5 loot proposal](LOOT_DESIGN.md) and [designer/critic review](LOOT_REVIEW.md): user requested one themed powerful item per chanter at about15%. Proposed Veiled bow, Bound shield, Resonant hook and Hollow neck clasp each have an independent15% eligible source-death roll, including partial kills. V4's shared completion power pool/first-world guarantee is superseded; vanilla payouts/dressing/lamp remain. Exact effects/repairs/delivery and targeted partial-farming economics await user review/playtesting; no loot implementation approved.
- Normal versus modified defender/bell drops; replenishing enemies may otherwise be farmed indefinitely.
- Stability recovery from kills: retain vanilla values or adjust for event defenders? Gearfoot's base recovery is particularly large.

## Placement and arrival

- Inspect actual vanilla rift placement rules for target version; do not invent an arbitrary placement system and call it vanilla.
- Player distance, reachable terrain, large portal clearance, inaccessible/underground locations, and no-valid-position behavior.
- Does the portal close after arrival, linger, or disappear at encounter end?
- What happens if chanters are attacked during their emergence/setup?

## Reinforcement details

- Exact eligible entity variants and weights within the all-tier family pools, including which locust types are appropriate.
- Confirm whether specialist death removes its family across every source.
- The opening bell summoning only drifters was proposed to avoid bypassing specialist deaths; it is **not yet approved**.
- After the initial four counted defenders plus bell, when does filling toward eight begin if nothing has died? Death-triggered pauses and post-pause refilling are agreed, but the initial unfilled slots need an explicit rule.
- Final health-based pause formula, threat modifiers, randomness, and upper/lower bounds.
- Does killing the one-time bell also grant a shared pause? Does killing a chanter grant one? Current agreement specifies defender kills; these cases need clarity.
- What happens to existing guards after their specialist or the last chanter dies? No automatic guard cleanup has been agreed.

## Lifecycle and scope

- Geographic/player scope of the stability penalty and how other players experience the locked storm.
- Is the 15-minute absence countdown shared by the group or per chanter? What resets it?
- Count real elapsed seconds versus game-calendar time explicitly; ensure unloaded chunks cannot prevent expiry forever.
- Does complete despawn end the storm immediately or just let the normal timer finish? Intent is that leaving gives a roughly 15-minute prolonged storm and cannot cause a permanent lock.
- Death/respawn, disconnect, reload, and later return to encounter site.
- Multiplayer: one shared procession per storm versus player-separated encounters. The current single-player-oriented concept is one procession per heavy storm while allowed; do not silently scale this per player.
- World-wide scope of the earned event control is selected; that does not settle who can operate it or the geographic/player scope of an active encounter's stability drain.

## Art and planning

- Review the [DLC mob concept sheets](VISUALS.md): Relief Engine chassis/arm, visibly empty Stretcher, and four defender silhouettes. Confirm proportions, anatomy, materials, and how closely to follow these illustrations before producing models; the art and panel labels do not independently approve combat or terrain-damage behavior.
- Confirm the working assignment: Bound/shivers, Veiled/bowtorns, Resonant/locusts, Hollow/bell.
- Final proportions, textures, chant, portal audio, healing cues, and animations.
- Final mod ID, build/API dependencies, license, source architecture, tests, and packaging when implementation planning is requested.
