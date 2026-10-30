# Loot design — designer/critic review log

**Joint recommendation — awaiting user review.** Updated 2026-10-26. Current package: [LOOT_DESIGN.md](LOOT_DESIGN.md), **V5 source-specific signatures**. V3/V4 history and sign-offs below apply to their former packages; V5 review and version-specific sign-offs are recorded at the end. Agent agreement does not approve gameplay changes. No implementation, installation, world edit, publishing, or gameplay verification occurred.

## Participants and evidence

- Designer/coordinator: main session; read the pasted brief, AGENTS, context/design/open questions, mechanical research, visuals, lore direction, collectible drafts, and relevant lore ledger/coverage. Inspected installed rewards, medicine, materials, lighting, recipes, repair and trade definitions; retrieved developer public stability and temporal-gear source. Only coordinator edits shared final documents.
- Independent critic: collaboration subagent `loot_critic`, not a sidebar chat. Independently read project/brief/lore constraints and inspected installed version, creature rewards/recovery, locust conversion, medicine, lighting, recipes and trader comparisons. It did not merely endorse the coordinator's evidence summary. No shared final-document edits by the critic.
- Installed executable FileVersion/ProductVersion independently confirmed as1.22.7. Base assets, public master source, original fiction, proposed behavior, and untested effective/runtime behavior remain separate in the proposal's evidence ledger. P7–P8 Archives exclusivity and tentative first-three visual/family assignments were retained throughout.

## A — goals and broad candidate pool

Reward a dangerous approach immediately, make full victory better than partial harvesting, preserve useful routine value after collecting trophies, and prevent indefinite event guard loot/stability income. Assume prepared late-copper/bronze onward, without adding an entry gate. Frequency and real combat expenditure remain unknown; use completed events as the balance unit.

The following were considered **before selecting the roster**, not offered as approved requirements. Some are rejected sketches, not fully researched item specifications.

| Candidate/use | Reason considered | Disposition |
|---|---|---|
| Rusty gears | Flexible ordinary reward independent of special recipes | Keep a guaranteed floor plus small optional rolls |
| Temporal gear | Existing useful temporal reward; completion/repeat motive | Keep one per eligible victory, no chanter rare rolls |
| Plain cloth/twine | Useful repair/crafting material fitting stays and veils | Keep modest amounts; distinguish cloth item from linen block |
| Clean bandages / ordinary poultices | Immediate recovery and saved supply effort | Keep two clean bandages; poultice stays a vanilla comparison |
| Alcoholed bandages | Strong existing medicine | Reject as stored routine reward: declared drying transition undermines dependable value |
| Rope bundle | Rescue stays and practical travel resource | Reject as major reward: cheap reed/papyrus/vine routes, weaker practical appeal |
| Copper/bronze metal bits | Compact smeltable useful salvage | Remove V1 amounts; use bounded mechanical salvage instead of alloy income |
| Metal parts/scraps | Existing maintenance salvage, useful later recipes | Keep on tender and finite bell; no Jonas randomizers |
| Finished tools/weapons/armor | High obvious value | Reject selected package: uncertain wear/repair/progression value, risks eclipsing crafted gear |
| Tuned knife | Specialized salvage tool | Reject extra equipment and overlapping tool role; not a developed final spec |
| Universal mechanical repair clamp | Recover armor/tool expenditure | Reject: repair recipes are material-specific; universal restoration erases that economy |
| Stability draught / stability field | Thematically obvious temporal reward | Reject: risks trivializing instability and event drain; no repeating stable sanctuary |
| Anti-storm wearable / four-piece set | Silhouette-specific strong prizes | Reject blanket defense, invented slots/set chores, and mandatory equipment progression |
| Return anchor / teleport relic | Rescue/return theme | Reject travel/death-cost bypass and larger persistence subsystem |
| Rare steel salvage | Endgame repeat motive | Reject free advanced-metal progression from guaranteed surface storms |
| Locator fork / Survey Listener | Find nearby translocators | Remove after critique: repaired targets usually already known, hidden/unloaded search scope excessive |
| Fueled Call Lamp | Portable thematic exploration light | Replace: V12/30-minute fat consumption was weak against existing lights and added chores |
| Signal cylinder / trophy | Rescue-call collectible and base furnishing | Reject new audio/recording content and extra collectable pool for this small package; no P7–P8 cylinder drops |
| Rescue dressing | Short application exposure, finite reusable-event incentive | Keep as one consumable per victory; revised healing interactions |
| Permanent Listener's lamp | Recognizable salvage furnishing plus useful backup light | Keep20% per victory; dimmer than vanilla lanterns |

Mostly vanilla is assessed by the guaranteed practical payout and verified purchase-cost comparison, not by counting names. All predictable economic value works without any novel crafting material or unique recipe.

## B/C — review cycle 1: V1 to V2

**V1 exchanged:** each chanter2 rusty plus an independent25% roll for1–2 more. Caller cloth; holder bandages/twine; tender8–12 bronze bits+fat; listener8–12 copper bits. Completion4 rusty+1 temporal gear; one mutually exclusive unique draw:40% dressing,20% locator,20% fueled lamp,20% none. Bell1 rusty+1 parts+2 scraps with0.05 recovery; guards zero rewards/recovery. The early dressing sketch was6HP/2s application/10s effect, not the final tick schedule. Lore notes remained world discoveries.

| Critic objection / concrete revision | Coordinator response and resulting change |
|---|---|
| V12 fueled lamp loses to vanilla torch/oil/lantern choices; propose permanent V16 trophy/light without fuel chores | **Accept.** Remove fueled lamp entirely. Listener's lamp V16 permanent; standard small/large lanterns remain brighter at18/20. No new fuel economy. |
| Locator for nearest active translocator is often redundant but requires world indexing/unloaded-target policy | **Accept/remove.** No locator retained. Existing exploration/travel gates unchanged. |
| Exclusive unique draw produces weak/empty repeats; propose guaranteed dressing plus independent20% lamp | **Accept.** Completion always supplies one dressing, lamp is separate bonus. Small roster of two; duplicate lamps allowed. |
| Dressing needs exact heal timing, activation failures, cooldown and stacking; do not replace best poultice | **Modify.** Six1HP ticks at2..12 seconds,2s application,15s cooldown; lower total than best7HP poultice. Shorter vulnerable application, not faster overall healing. Initial V2 compatibility rule later revised below. |
| Free alloy bits are thematically thin; propose ordinary mechanical parts/scraps | **Accept/modify.** Tender gets1 parts+2 scraps; listener gets2 twine+fat instead of metal. No routine copper/bronze/iron/steel ingot income. |
| Bell0.05 recovery is finite, full0.15 could also be acceptable if persisted | **Simplify.** Set all event-origin kill recovery0, including bell/chanters, instead of another exceptional allowance. This is a user-review proposal, not accepted encounter design. |
| No-loot guard rule misses conversion into hacked locusts/carried creatures | **Accept.** Explicit event-origin capture/hacking refusal, persistent provenance and reward suppression across unintended conversion. Interaction restriction disclosed for user review. |
| Unbounded defender drops include large TG/Jonas awards, not merely fibers | **Accept with evidence.** Suppress initial guards and all replenishments, knife/rip/death/randomizer paths, surviving guards after victory; finite bell replaces its vanilla cylinder/Jonas rolls. |

## D — review cycle 2: V2 to V3

**V2 exchanged in full:** final vanilla rows and14.5 expected gears with bell; completion dressing1 and lamp20%; detailed unique activation/limits; proposed persisted shared loot ledger. Early V2 blocked all vanilla medicine during dressing ticks; conversion wording mixed refusal with retained conversion. Completion eligibility initially required direct player/projectile deaths only.

| Critic substantive objection / concrete revision | Coordinator response in V3 |
|---|---|
| Blocking vanilla healing denies a lifesaving input and requires surprising universal healing patches. Give vanilla priority: starting ordinary healing cancels the relic ticks | **Accept.** Starting vanilla medicine cancels remaining dressing healing, without refund; normal application proceeds. Dressing still cannot start over an existing vanilla healing-item effect. Natural regeneration unaffected. Unknown mod healers require compatibility review. |
| Refusing conversion and allowing permanent converted companions is contradictory | **Accept/clarify.** Normal event hacking/capture refused with feedback and no consumption; retained provenance through unexpected third-party transformations is defense in depth, not a supported companion-export path. No cap rule changed. |
| Direct-only credit forfeits completion on a recently fought target's fall/burn. Recommend a30-real-second player-damage grace window | **Accept.** Direct player/projectile or environmental death within30seconds of that target's last player damage qualifies. Time/credit persists across reload. All four slots still must qualify in the same event. Ineligible deaths still perform approved healing/storm-end behavior. |
| Lamp must not promise absence of ordinary block-light effects on spawning/growth | **Accept.** No special immunity/effect; normal engine light interactions remain. V16 is a brightness setting, not a measured radius. |
| Dressing6HP over12 seconds plus2s application is14s overall, slower than best poultice's declared13s | **Accept.** State short activation exposure as the advantage, explicitly compare total times. No instant tick. |
| Review sources precisely: vanilla `avg` values are not verified Bernoulli probabilities; locust header does not itself establish stability recovery | **Accept.** Ledger preserves vanilla declaration notation, proposal separately defines exact rolls. Event0 recovery policy does not claim every vanilla creature previously had recovery. |

The second review also checked final expected totals: four optional rolls produce1.5 expected extra rusty gears; four chanter guaranteed8+completion4+bell1 =13 minimum,14.5 expected. Supplies: cloth1/twine4/bandages2/parts2/scraps4/fat1/TG1/dressing1/lamp0.2. No additional hypothetical review rounds are claimed.

## E — historical V3 exact final review and sign-offs

The coordinator saved **V3 in LOOT_DESIGN.md** and asked the critic to read the actual full document, including the same tables, exact unique specs, quantities, odds, restrictions, and delivery rules. Review applies to that exact package, not merely the theme. The critic's final read found one persistence ambiguity: logout canceled dressing ticks, but shutdown also disconnects clients. The coordinator accepted its correction: intentional logout/network disconnect cancels ticks, while server/world shutdown preserves pending offsets and cooldown and pauses authoritative time. The critic then explicitly signed off the amended, otherwise unchanged V3. This was a final clarification, not an invented third substantive review cycle.

**Designer/coordinator — recommends V3.** The vanilla floor provides repeat value without component clutter; the two uniques have clear limited roles; event-owned farming is bounded without changing agreed combat. Remaining uncertainties are actual combat profitability, heavy-event frequency, existing natural-storm farming incentive, multiplayer scope, and implementation compatibility/persistence. They require user decisions and later tests, not further speculative balancing. No remaining designer objection to this proposed version.

**Independent critic — recommends exact saved V3, including the shutdown clarification.** Its returned sign-off explicitly agrees with every chanter's fixed drops and 25% equal 1–2 gear roll; completion 4 gears + 1 temporal gear + 1 dressing and independent 20% lamp; bell 1 gear + 1 parts + 2 scraps; zero guard loot and zero event kill recovery; both complete unique specifications; 30-second indirect credit; shared persisted once-only delivery/failure rules; zero event collectible drops and P7–P8 Archives exclusivity. **No remaining substantive objections.** The critic acknowledges the documented medicine/lamp compatibility, corpse-decay/lifecycle, crash/multiplayer verification, frequency/cost tuning and ordinary storm-mob farming uncertainties. Agreement is a joint recommendation awaiting user review, not user approval or measured balance.

## Remaining decisions and validation limits

- User must approve/change the package; especially zero event kill recovery, event hacking/capture refusal,30-second attribution, strict all-four completion qualification, and shared physical payout.
- Actual event frequency and reward-to-cost ratio are unmeasured. Existing natural storm mobs remain farmable; event-tag suppression alone cannot remove that wider incentive without altering other systems.
- First-three visual/family assignments, loot storage decay duration, encounter persistence/despawn handling, multiplayer stability scope, and ordinary-versus-modded interactions remain unresolved where the canonical design leaves them open.
- Potion/healer compatibility, guard transformations, ledger crash races, lamp lighting/state, save/reload, decay and multiplayer are future implementation gates. Neither agent claims gameplay validation or technical feasibility already proved.
- Documentation validation checks links, table arithmetic and contradictory numbers only. P1–P8 prose is untouched. No approved mechanic or lore identity was changed.

V3 documentation check: all **48 local Markdown/source links** across the two deliverables, README and current context resolved. Recalculation confirmed 13 minimum / 14.5 expected / 21 maximum rusty gears with bell, lamp mean wait 5 victories and 67.2% / 89.3% / 95.6% acquisition after 5 / 10 / 14 victories, and the explicitly hypothetical 83-day example. No executable tests/build exist; none were reported as run.

## F — user steering and V4 power brief

After reviewing V3, the user explicitly requested adding uniques with player power: the hardest temporal storm is becoming harder and the rewards should feel worth it. The earlier conservative rejection of all equipment and stability benefits is therefore **historical**, not a binding constraint on this revision. Keep the ordinary supply foundation, but allow earned combat and exploration advantages with explicit limits. The user has not approved exact items/numbers or gameplay implementation.

The same independent critic was reactivated using collaboration tools. Both agents rechecked installed weapon, shield, neck-slot, armor-stat and stability-handbook comparisons. Coordinator retrieved developer `ItemShield.cs` separately, with a public-master version caveat. Only the coordinator edited final documents.

Additional broad power candidates included a timed melee-speed consumable, bow-spread benefit, extra maximum-health amulet, lifesteal weapon, generic damage resistance, hunger efficiency and knockback pulse. We selected specialist damage, a periodic strong block, and modest sustained stability retention instead. The rejected alternatives were less tied to the apparatus, broadly useful without distinct limits, or introduced on-kill farming/control loops. This was not a claim that every rejected effect is impossible to balance.

## G — V4 review cycle 1: V4a to V4b

**V4a exchanged:** retain all V3 vanilla/dressing/lamp rewards and add three completion power relics: a 5-base/6.25-specialist iron-tier hook, a metal shield with +3 absorption on its first strong block per 8 seconds, and a neck clasp multiplying environmental drain by 0.85. Initial acquisition sketch guaranteed one random relic every victory.

| Independent objection or concrete revision | Coordinator response |
|---|---|
| Guaranteed permanent power every victory can dominate expected early utility; do not claim a mostly-vanilla practical-value majority based on name counts | **Accept.** First world victory guarantees one relic; later victories have a 50% chance of exactly one, each of three equally likely. One persisted world flag prevents per-player/alt guarantees. State honestly that an early relic may be the most valuable reward; mostly-vanilla describes routine supplies and material economy, not a fabricated utility percentage. |
| Shield's extra absorption can be wasted on a weak hit already fully blocked | **Accept.** Trigger only on successful active block with positive residual after base 5/10 absorption; absorb min(3, residual). Even a 1-damage save spends the cooldown. No trigger on passive/failed/full-base blocks. |
| A generic temporal/mechanical tag can include unrelated bosses, mod mobs or friendly helpers | **Accept.** Hook gets an explicit base-family variant allowlist plus the four mod chanters, excluding owned/hacked/friendly targets, players, bellmini and unrelated bosses. No aura or buff to another weapon. |
| Apply the weapon bonus to the weapon contribution once, before target defenses; do not promise raw 6.25 outperforms steel against every armor tier | **Accept.** Base contribution 5×1.25; ordinary character modifiers afterward once. Keep iron tool-tier/damage handling and 850 durability, compare steel's tier 5 and 2125 durability as well as 5.25 base power. |
| Blade assets include Sharpenable/Buffable; define those rather than overlook more stacking | **Modify.** Hook excludes item sharpening/quenching/buff recipes; ordinary character modifiers still apply. This keeps a clear fixed specialist contribution and preserves an advantage for upgraded crafted steel. |
| Clasp needs a precise stage/source definition; reducing net stability change would also change regeneration/recovery | **Accept.** Scale covered standard environmental loss components by 0.85 after Procession multipliers and before adding unchanged positive gains; exclude device/direct costs, kill recovery and unknown mod drains. No claim that a stock resistance stat exists. |
| Neck-slot competition is a weak cost; do not dress it up as a major disadvantage | **Accept.** Clasp is intentionally a durable equipped upgrade with a finite 15% reduction, not immunity. No charge/fuel chore. Acknowledge the minor opportunity cost and 17.6% longer constant-drain budget. |

## H — V4 review cycle 2: V4b to saved V4

**V4b exchanged:** full concrete specifications for all three items, exact ordinary and bonus absorption, timers, weapon targeting/repairs, clasp loss-stage rules and corrected acquisition/expected yields. No V3 economic/guard/lore rules removed.

| Independent substantive objection or precision fix | Coordinator response in saved V4 |
|---|---|
| Do not leave shield wear as unspecified normal behavior when its implementation was not inspected | **Accept.** Explicit custom wear: 1 per successful block, +1 on a boosted block, none on failures/idle, independent of absorbed HP. Compare declared durability only; do not infer vanilla-equivalent hit lifespan. |
| Game-calendar timers could recharge by sleeping/time commands | **Accept.** Shield's 8-second cooldown uses active-server real elapsed time, persists through swaps/death/logout/reload, shutdown pauses; calendar speed and sleep cannot accelerate it. |
| Hook block-breaking wear wording leaves tool function and charges ambiguous | **Accept.** Weapon only, no block-breaking/harvest function; exactly 1 durability per landed melee attack, none on a miss/cancel. |
| Retained broken equipment and shapeless repair can create duplication/full-refill paths | **Accept.** Zero-durability relics are inert but accept the same repair. Consume original as the same logical instance, return exactly one item, add fixed 250 capped at maximum, preserve attributes/cooldowns; no recipe without original, no extraction. Tests required for retry/stack/race handling. |
| Clasp removal must not award stored or retroactive protection | **Accept.** Only subsequent covered losses while worn, immediate stop on unequip, no refund, no copies in other slots/inventory, recompute after reload rather than add repeatedly. |
| The world first-win flag must be consumed at victory, not harvest/ownership | **Accept.** Atomically commit flag with eligible final-death reward record; an unclaimed/lost relic does not reopen guarantee. Shared reward, no per-player compensation. |

Critic independently confirmed acquisition arithmetic: specific relic mean 5 wins, all three mean 10; total expected count after n wins is 1+0.5(n−1). It reported no further balance objection to the 0.85 clasp or 5/6.25 hook after these fixes. That was support for the revised direction, not yet exact-document sign-off.

## I — historical V4 exact-document sign-offs

Coordinator saved the full V4 specification, completion tables, unchanged V3 payouts, all five item specifications, acquisition calculations and evidence ledger, then requested an independent read of that same file. V3's signatures do not establish agreement on these additions.

**Designer/coordinator — recommends exact saved V4.** First-victory gear makes the first completion memorable; later 50% drops make a credible equipment chase. The hook improves specialist offense, Braceguard adds meaningful strong-hit protection, and the clasp extends stability budget without eliminating instability. Their exact limits keep crafted steel, facing/armor, medicine and temporal gears relevant. Vanilla supplies remain routine economic value. No remaining substantive designer objection; encounter cost, frequency, mod compatibility and implementation behavior remain untested.

The exact-document read found two final precision issues. **Accept:** the bell target is `game:bell-normal`, generated by `bell.json`'s normal variant. The coordinator initially challenged this after bounded output omitted lines 16–18; a full targeted read confirmed the critic's source evidence, so no source-view discrepancy remains. **Accept:** the clasp cannot be described as never turning falling stability into rising stability when simultaneous positive recovery is left unchanged. V4 now explicitly allows the mixed-source example (loss 1/recovery 0.9 becomes net +0.05 after attenuation) and keeps the intended per-loss effect without a new net-drain clamp. These are final source/arithmetic corrections, not an invented third substantive review cycle.

**Independent critic — recommends exact saved V4 after those corrections.** It freshly reviewed the same full document and explicitly agreed with all five unique specifications; hook 5/6.25, tool tier 4, 850 durability, 250-durability iron repair and explicit bell-normal targeting; shield 5/10 plus situational 3 extra absorption per 8 active-server real seconds, 500 durability/custom wear/250 repair; clasp covered-loss ×0.85 including the stated mixed-recovery consequence; first-world guaranteed uniform relic and later 50% uniform pool; unchanged chanter/bell/vanilla/dressing/lamp tables and accounting/farming/lore boundaries. **No remaining substantive design or numerical objections.** It acknowledges the intended early unique power value and vanilla routine supply economy. This is a joint recommendation awaiting user review, not implemented, tested, or user-approved behavior.

V4 validation: all **75 local Markdown/source links** across the proposal/review/current context/design/open questions/README resolved. Recalculation confirmed hook base 6.25, constant-drain time gain 17.647%, expected 5.5 relics after ten victories, about 1.83 of each, and expected ten victories to collect all three types. No gameplay test/build was run or claimed. Original lore prose and accepted encounter values were preserved.

## J — V5 brief and review cycle 1

2026-10-26 user request: a themed powerful item from each “miniboss,” powerful enough to justify about a 15% drop rate without breaking the game. The coordinator interprets this as one fixed signature for each existing passive chanter, independently rolled at its eligible death. It supersedes the proposed completion power pool/first-world guarantee; it does not turn the chanters into attacking bosses or approve their precise rewards. Routine vanilla rewards and the separate completion incentive remain.

The independent critic rechecked installed bow/arrow stats and recipes, including class gates, and computed acquisition consequences. Coordinator separately retrieved developer public `ItemBow.cs`, distinguishing its master version from installed assets. V4's three power values were retained: rarity is not an automatic reason to inflate a sound effect. A new **Last Call bow** from the Veiled adds prepared ranged offense; alternatives of a voice-lure whistle, generic veil buff or detection wearable would require more unrelated behavior or diminish encounter threat.

**V5a exchanged:** Veiled → Last Call bow; Bound → Braceguard; Resonant → Breach hook; Hollow → Listening clasp, exactly 15% independent per source kill, no completion power roll/first-win guarantee. Bow baseline longbow 3.75/+0.2 accuracy/tier0/750 durability; 25% more bow+arrow contribution on an uninterrupted 2.25-second draw if its 6-real-second cooldown is ready; normal ordinary shots remain available. Fixed repair and boosted-shot wear proposed. Other three relics retain V4 numerical effects.

| Independent critique / concrete revision | Coordinator response in saved V5 |
|---|---|
| Any-class use must not erase bowyer/ordinary character ranged traits | **Accept.** Ordinary character modifiers apply once after bow/ammo contribution; any class can use/repair the relic, no trait or recipe unlock. Longbow baseline avoids copying the gated recurve recipe. |
| Swapping equipment after release must not change the shot or multiply its bonus; release accounting can duplicate projectiles/ammo | **Accept.** Lock contribution/precision at projectile emission, server-validate and atomically account arrow/wear/cooldown/single projectile. No precision state on recoverable ammo; misses still spend wear/cooldown. |
| A fixed minimum firing threshold must not bypass other normal bow cancellation conditions; render-full is not full accuracy | **Accept.** 2.25 seconds is a custom hold from draw start; all stock firing eligibility/cancels retained. Public source's 0.65 minimum is evidence, not a promise every later release is valid. |
| Rare bow needs repair/broken-state exactness rather than disposable power or duplicated repairs | **Accept.** 750 max, 1 wear ordinary/2 precision; broken inert item retained, custom original+2 twine+1 resin repair restores250 capped at750, same identity/no extraction/item buffs. Not described as stock bow repair. |
| Passive chanters must not be removed by hook's hostile-target filter | **Accept.** Explicitly include the four nonattacking chanters; exclude owned/friendly ordinary creatures without requiring aggression from chanters. Their shared healing/50% ranged resistance stays intact. |
| New source-specific acquisition contradicts old no-power-on-partial and first-world ledgers | **Accept.** Save each signature's success/failure at its source death, partial payouts stay earned, completion gives no signature; no first-world flag/pool. Full clear may drop0–4, not a shared60% choice. |

## K — V5 cycle 2 and exact-document review

The coordinator saved the full V5 document for a second independent review: six uniques including four source signatures, all original vanilla/dressing/lamp rewards, fixed15% rolls, unchanged three older power effects, complete new bow specification, persistence/partial-kill rules, sources and recalculated acquisition. The critic read the actual same file rather than carry forward V4 signatures. It checked the full tables, all six specifications, source caveats, lifetime/repair/charge rules, partial entitlement and removal of the old pool. It found no new substantive defect; no disagreement was manufactured simply to increase the round count.

**Designer/coordinator — recommends exact V5.** The four reward identities now follow each figure's work: caller's deliberate shot, holder's reinforced brace, tender's cutting tool, listener's stability reference. A 15% find is useful beyond this encounter, repairable where it wears, and constrained by ordinary ammunition, iron tier, shield facing/cooldown or finite personal drain reduction. Completion retains meaningful vanilla/medicine value even when rare rolls fail. No remaining substantive objection; actual difficulty, acquisition frequency, targeted partial-farm profitability and runtime compatibility remain untested.

**Independent critic — recommends exact saved V5.** It explicitly agrees on all four15% independent source rolls, zero-to-four per clear, no first-world/completion power pool, and per-source partial entitlement; unchanged vanilla/completion/dressing/lamp/farming/Archives rules; and every specified effect, quantity, probability, limit, repair and delivery rule. It independently confirmed bow contribution7.1875 with iron/7.8125 with steel ammunition, source longbow/recurve comparisons and acquisition arithmetic. It considers the bow's2.25-second preparation,6-second boosted cadence, ordinary defenses/ammo/class modifiers and double wear on precision shots desirable without overriding survival combat. Existing hook/shield/clasp are already meaningful power and need no automatic rarity-driven buff. **No remaining substantive objections.** Joint recommendation awaiting user review; runtime difficulty, compatibility, multiplayer and actual hunting costs/frequency remain untested.
