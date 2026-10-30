# Lore research ledger — The Procession

Research begun 2026-10-24 (America/New_York). **Pass 1: foundation and source inventory.** This is a research record, not an approved creature identity or mod lore draft. Major historical spoilers are isolated below. See [coverage](LORE_SOURCE_COVERAGE.md) for exactly what was read and what remains.

## Roadmap and stopping points

1. **Foundation — begun here:** verify the installation; read the handbook's account of stability/rifts/storms; index collectible texts and dialogue; compare historical witnesses and the original purposes of machines. Record uncertainty before proposing explanations.
2. **Connected evidence — next:** finish the remaining collectible groups; trace relevant dialogue branches through their English strings, especially Tobias, the named settlement characters, and the resonance/story encounters. Cross-check artifacts against their actual placement, models, paintings/tapestries, and encounter context. Investigate drifters, shivers, bowtorns, locusts, bells, the Rot, seraphs, prima materia, the Lens, and the Machine without assuming one explanation covers them all. Check official versioned developer material and wiki citations where useful.
3. **Synthesis — after that evidence:** compare several genuinely different interpretations of the four beings. For each, separate support, original invention, contradictions, and unanswered questions. Recommend a direction for discussion; do not make it established without the user choosing.
4. **Collectibles — after direction is chosen:** plan authors, dates/relative eras, motives, knowledge limits, placement, distinct voices, and a gradual reveal. Label all drafts as original mod writing. Observable encounter warnings can be accurate while their authors misunderstand the cause.

These passes need not exhaust every asset in the game, but must account for relevant material and explicitly carry forward gaps. No creature names or finished collectible texts have been proposed in this pass.

## Evidence rules

- **C — Explicit canon:** distinguish **C-system** (handbook or other direct game statement) from **C-testimony** (the game contains this account, belief, memory, or legend). A canonical document is not automatically a reliable account of objective history.
- **I — Strong inference:** multiple identified observations support a reading, but do not explicitly establish it. State alternatives and scope.
- **U — Unresolved/contradictory:** missing corroboration, conflicting sources, unclear attribution, or an untested causal connection.
- **K — Community interpretation:** an external reader's theory, identified as such; not upgraded by repetition or publication on the official forum.
- **O — Original addition:** our encounter, interpretation, or writing. Separate approved mechanics from future proposals.

Asset configuration establishes what the base files declare, not tested runtime behavior or the final mod-patched state. Internal category names and identifiers help attribution and tracing; they are not themselves player-facing proof of an ontology.

## Version, language, and method

- **V1:** private local client-main.log (not distributed), opening three lines: log starts `24.10.2026 21:35:44`, reports `v1.22.7 (Stable)`, and names `E:\Games\Vintagestory\Vintagestory.exe`. This corroborates the installation used by that client session, not the latest public release or a clean-install audit. The log is mutable and may be replaced next launch.
- `E:\Games\Vintagestory\assets\version-1.22.7.txt` is present. It returned no useful text; its filename is supporting evidence only. The executable's queried FileVersion/ProductVersion fields gave no useful version string.
- This pass reads **installed English base assets**. It does not establish that English is the original authoring language, compare translations, inspect every patch/mod, or verify which strings are reachable in a particular save. Preserve unusual terminology instead of silently reconciling it.
- No launch, gameplay test, save access/change, game-file edit, mod installation, decompilation, or publication was performed.
- Agent-reach's prescribed Exa search was attempted through `mcporter`; DNS resolution of `mcp.exa.ai` failed. Available web browsing supplied official-site and wiki reads. No social-platform login or community corpus was needed for this pass.

## Source register

Local text source **EN** throughout is [assets/game/lang/en.json](E:/Games/Vintagestory/assets/game/lang/en.json). Each ledger entry identifies the exact keys. Lore definition files establish the connection between a collectible code and those keys; definitions use relaxed JSON syntax. Their directory is [config/lore](E:/Games/Vintagestory/assets/survival/config/lore).

| ID | Source and exact locator | Evidence role / limit |
|---|---|---|
| L1 | [13-temporalstability.json](E:/Games/Vintagestory/assets/survival/config/handbook/13-temporalstability.json); EN `gamemechanicinfo-temporalstability-title`, `gamemechanicinfo-temporalstability-text` | Direct player handbook statement; broad explanation, not an exact threshold audit. |
| L2 | [reflection.json](E:/Games/Vintagestory/assets/survival/config/lore/reflection.json); EN `lore-reflection-piece1` through `piece5`; category `tobias` | Friendly retrospective on Jonas; sympathetic witness with limited access to his private discoveries. |
| L3 | [breakdown.json](E:/Games/Vintagestory/assets/survival/config/lore/breakdown.json); EN `lore-breakdown-piece1`; category `jonas` | Distressed first-person report of the Lens, another world, and an eidolon incident. |
| L4 | [confession.json](E:/Games/Vintagestory/assets/survival/config/lore/confession.json); EN `lore-confession-piece1`; category `jonas` | First-person guilt and religious language; not proof of divine intervention. |
| L5 | [dmetrisnotes.json](E:/Games/Vintagestory/assets/survival/config/lore/dmetrisnotes.json); EN `lore-dmetrisnotes-piece1` through `piece5` | Dmetri Arvo's notes; elitism, fear, and a changing appraisal of Falx. |
| L6 | [dietofkings.json](E:/Games/Vintagestory/assets/survival/config/lore/dietofkings.json); EN `lore-dietofkings-piece1` | Scribe's account headed 1318, recording Falx's medical/political presentation. |
| L7 | [letters.json](E:/Games/Vintagestory/assets/survival/config/lore/letters.json); EN `lore-letters-piece1` through `piece8` | Multiple voices: quartermaster, workers, family correspondence, infected observer, military order. Do not merge into one narrator. |
| L8 | [tapestry.json](E:/Games/Vintagestory/assets/survival/config/lore/tapestry.json); EN `tapestrylore-*`, all 18 keys listed by the definition | Interpretive first-person commentary, sometimes quoting device descriptions. Narrator's identity/trigger context not independently traced in this pass. Images not inspected. |
| L9 | [stone.json](E:/Games/Vintagestory/assets/survival/config/lore/stone.json); EN `lore-stone-piece1` through `piece9` | The Weight of Stone: transcribed recording with stated losses and archival commentary. |
| L10 | [patronage.json](E:/Games/Vintagestory/assets/survival/config/lore/patronage.json); EN `lore-patronage-piece1` through `piece5` | Nobleman's recordings, conversation, and a later postscript; different knowledge layers. |
| L11 | [return.json](E:/Games/Vintagestory/assets/survival/config/lore/return.json); EN `lore-return-piece1`, `piece2`; category `tobias` | Survivor's retrospective, including a claim of centuries of work; independent chronology not established. |
| L12 | [themorning.json](E:/Games/Vintagestory/assets/survival/config/lore/themorning.json); EN `lore-themorning-piece1` through `piece5`; category `villager` | Oral origin story about Bearfirth; myth and community instruction. |
| L13 | [blindbatrickhart.json](E:/Games/Vintagestory/assets/survival/config/lore/blindbatrickhart.json); EN `lore-blindbatrickhart-piece1` through `piece8` | Cautionary tale with a threatening final address; not a machine identification guide. |
| L14 | [wallcarving.json](E:/Games/Vintagestory/assets/survival/config/lore/wallcarving.json); EN `wallcarvinglore-alain`, `-demir`, `-ors`, `-heloise`, `-tobias`, `-unknown` | Epitaphs and a visionary inscription; actual wall placement not traced. |
| L15 | [trader.json](E:/Games/Vintagestory/assets/survival/config/dialogue/trader.json), component `drifters`; EN `dialogue-miserablesouls` | Trader explicitly supplies a religious explanation; adjacent player choices are not trader testimony. Full dialogue review pending. |
| A1 | [bell.json](E:/Games/Vintagestory/assets/survival/entities/lore/bell.json), `tags`, `attributes.isMechanical`, server task `bellalarm.spawnMobs` | Mechanical tag and a mixed reinforcement list in base config. |
| A2 | [locust.json](E:/Games/Vintagestory/assets/survival/entities/lore/locust.json), `tags`, `attributes.isMechanical`, `variantgroups` | Mechanical tag; bronze/corrupt/corrupt-sawblade variants. Appearance and code labels do not fully explain origins. |
| A3 | EN `itemdesc-gear-temporal`, `itemdesc-spear-generic-hacking` | Item descriptions; latter includes a metaphorical line about children. Metaphor is not genealogy. |
| W1 | [Official game overview](https://www.vintagestory.at/) | Developer-published framing: eldritch horror, ruined civilization, player reconstruction from fragments. Not a technical cosmology. |
| W2 | [Official story excerpt: The Morning](https://www.vintagestory.at/stories/storyexcerpt-themorning.html/) | External official fiction, read alongside L12. Publication/version alignment not established. |
| W3 | [Official story excerpt: Ghosts](https://www.vintagestory.at/stories/storyexcerpt-ghosts.html/) | External official fiction; full excerpt read. Its precise chronology and relation to a particular in-game collectible remain unverified. |
| W4 | [Temporal stability wiki](https://wiki.vintagestory.at/Temporal_stability), rendered oldid `413186` | Supporting index/mechanics overview; exact last-edited/version verification dates not exposed by successful fetch. |
| W5 | [Temporal storm wiki](https://wiki.vintagestory.at/Temporal_storm), rendered oldid `309280` | Supporting index; contains conflicting advice and a numerical inconsistency; see U3. |
| W6 | [Temporal rift wiki](https://wiki.vintagestory.at/Temporal_rift), rendered oldid `327703` | Supporting index; links the 1.16 introduction. Linked release statement not yet reviewed. |

Web pages were accessed during this pass on 2026-10-24 client date. Rendered revision IDs were reported by the page, **not successfully reloaded as pinned revisions**. Attempts to retrieve wiki page-info dates and some Rust World page URLs failed. Search crawl dates are not page revision dates. No reviewed wiki citation establishes a definitive origin of the Rust World.

## First findings — limited spoilers

| ID | Class | Finding | Evidence and boundary |
|---|---|---|---|
| C1 | C-system | The handbook explicitly calls rifts portals to the rust world, associates proximity with rapid stability loss and drifter spawning, and describes storms as having the same effect as low stability. | L1, EN `gamemechanicinfo-temporalstability-text`. This is firmer than treating the portal connection as a fan theory. It does not explain the realm's origin. |
| C2 | C-system | The handbook connects deep unstable areas, a depleted stability meter, dangerous creatures, and entry into the rust world. It describes killing drifters and consuming a temporal gear with a knife as ways to recover stability. | L1; A3 `itemdesc-gear-temporal`. Exact numerical thresholds/effective runtime behavior not checked. |
| C3 | C-testimony | The trader calls drifters cursed, grieving souls and invokes divine punishment. | L15. Canon establishes that this trader says this; it does not independently establish their former identity, sin, or a deity's involvement. |
| C4 | C-testimony | Bearfirth's story combines a changed world, wheels/storms, nocturnal threats, a divine bargain, and survival through collective work. | L12 pieces 1–5; W2. Read as narrated tradition, not a literal scientific account or universal religion. |
| I1 | Strong inference | Fragmentary testimony, practical hardship, kindness, prejudice, and uncertainty are central tools of the setting's storytelling. Horror need not dominate every sentence. | L7, L9, L10, L12, L13; official framing W1 and dramatic example W3. This is a tonal reading, not a formal developer rule. |
| U1 | Unresolved | Calling the realm the past, the future, a failed timeline, an afterlife, or the inevitable result of a single machine exceeds this pass's evidence. | C1–C2 establish a connection, not a complete causal history. Historical warnings below do not close that gap. |

<details>
<summary><strong>Major historical/story spoilers — witnesses, technologies, and institutions</strong></summary>

| ID | Class | Finding | Evidence and reliability |
|---|---|---|---|
| C5 | C-testimony | Tobias's account credits Jonas with locusts and eidolons after an unexplained breakthrough, before the later crisis assembly. | L2 piece4. He explicitly lacks Jonas's private explanation. This is strong evidence of an in-world attribution, not a complete mechanism. |
| C6 | C-testimony | Tapestry commentary describes a small digging/material-handling helper, a bell for communication/recording/help, and a mobile colossus for scaffolding and hoisting. | L8 `tapestrylore-schematic-a`, `-b`, `-c`. Do not silently equate every quoted design with every modern creature variant. |
| C7 | C-testimony + asset corroboration | Bells were represented as useful recording and rescue devices, as well as disliked watchers. | L8 `tapestrylore-schematic-b`; L9 piece5 invokes a needed bell after an accident; L10 piece1 identifies the purchased recording contraption as a bell. A1 independently marks today's entity mechanical. |
| C8 | Base asset fact | The installed bell's reinforcement list contains drifters, a shiver, a bowtorn, and a corrupt locust. | A1 `bellalarm.spawnMobs`. This supports an existing mechanical association of those families; it does not say whether help arrives by command, attraction, creation, or a portal. |
| C9 | C-testimony | The Jonas-category Breakdown text describes looking through the Lens, involuntary shifts into another world, perceived leakage, and concern about otherworldliness in his creations. An eidolon is reported bloodied; workers call it an accident. | L3. The narrator believes the cause goes beyond an accident. His distress does not justify dismissing the report, but his diagnosis is not omniscient confirmation. |
| C10 | C-testimony | Prima materia is described as a transformative discovery; another-world warning urges caution about changes to this world. | L8 `tapestrylore-schematic-d`, `tapestrylore-temporality`. Neither gives a verified recipe, a full physical theory, or proof that all supernatural creatures share one origin. |
| C11 | C-testimony | The Rot crisis is described through plant blight, human illness, devastation, failing supplies, and a proposed Grand Machine. | L5 pieces1,3,4; L6 piece1; L7 pieces1,4,6,7. L6 is a scribe's account of a presentation, not an independent clinical authority. L5 piece4 records the decision to send materials. |
| C12 | C-testimony | Falx's public elevation as a savior coexists with private guilt and religious appeal; social collapse persists around the project. | L4 and L8 `tapestrylore-holy`; contrast L2 piece5. Faith exists in these accounts, but neither automatons nor the proposed chanters thereby become worshippers. |
| C13 | C-testimony | The sources name Caydehill University, a rector, Lord Chamberlain Folse, Ingmar Falx, an emperor and a Diet of Kings; other texts show military authority, underground work, and stratified access. | L5 pieces1–4; L6 piece1; L7 piece8 (Marshal Durick Lawrence); L8 `tapestrylore-blackguard`, `-gate`. Titles and specific political geography require later cross-checking. |
| C14 | C-testimony | The Return narrator describes awakening injured after the black sickness, a changed surface, and subsequent centuries of work. The seraph tapestry voice expresses uncertainty about returning. | L11 pieces1–2, category `tobias`; L8 `tapestrylore-seraphim`. Do not collapse these accounts into a complete explanation of every seraph. |
| C15 | C-testimony | Epitaphs name individuals and places; a separate unknown inscription speaks of gears, the Firmament, and perceived revelation. | L14. Visionary or religious language does not prove literal cosmic machinery or identify a supreme entity. |
| I2 | Strong inference | Old technical functions persist in corrupted or dangerous forms, making the relation between useful machines and present threats a promising question. | C5–C9 and A1–A2. Exact causes and extent remain unresolved; this does not establish that every present threat was manufactured. |
| I3 | Strong inference | Accounts of Rot and accounts of otherworldly technological risk should be compared as related strands, without assuming they are interchangeable. | L2 piece4, L3, L5, L6, L7. The material supports both an epidemic crisis and concern over the response; it does not settle a single causal chain. |

The official Ghosts excerpt additionally portrays suspicion, bereavement, a costly act of help, and a blue stranger's apparent return after death (W3). It is useful for tone and comparison with return narratives; identifying that stranger or deriving a general resurrection mechanism would go beyond the excerpt.

</details>

## Unresolved and contradictory evidence

- **U2 — Useful/hostile bells:** source accounts corroborate recording and rescue roles; current config declares hostile reinforcements. How those roles connect historically is not explained by reading a spawn list. Do not label bells simply demonic summons or assume every reinforcement is materialized from nothing. [L8–L10, A1]
- **U3 — Wiki mechanics conflict:** W4 says slabs prevent storm spawns, while W5 says collision clearance can allow spawning on slabs. W5 also equates 28m48s with 2.99% of a day, although it is 2%. W4 and W5 give differing headline frequency ranges. Do not choose numbers from these pages as authoritative 1.22.7 evidence. This pass does not resolve runtime behavior or change [DESIGN.md](DESIGN.md).
- **U4 — Narrative attribution/context:** lore categories support Jonas/Tobias grouping, but tapestry narration, acquisition context, chronological ordering, and story-location staging still need tracing. Do not assign every first-person sentence to Jonas or Tobias without evidence.
- **U5 — Creature coverage:** bell/locust header and relevant bell-task inspection are partial. Detailed origins of drifters, shivers, and bowtorns are not established here. The prior [mechanics research](RESEARCH.md) is not a substitute for their narrative evidence.
- **U6 — Chronology:** a dated scribe record and a claim of centuries do not produce a complete calendar. Do not derive a present year, exact Rot duration, or creation order beyond the text's relative sequence. [L2, L6, L11]
- **U7 — Material/sound metaphysics:** bells, recordings, tuning cylinders, and names involving resonance invite investigation. They do not prove that chanting can heal companions, sustain storms, or manufacture organisms. The visual tuning fork is our design, not recovered canon.
- **U8 — Translations and evolving content:** only English was read. No comparison of alternate language terms for Rot/rust, soul/seraph, Machine, or Lens; no comprehensive historical release comparison. Wiki dates and external fiction's precise version alignment remain unavailable/unverified.

## Community interpretations

**No community theory has been admitted as evidence in pass 1.** Broad search results surfaced interpretations, but their underlying posts and support were not reviewed. Future entries must name/link the author/post, distinguish the theory from any primary quotations, and test it against installed texts. An official-hosted forum post is still community interpretation unless its author's developer role and statement are verified.

## Our original additions and boundaries

- **O1 — Approved encounter design:** four passive, visually distinct chanters emerging during heavy storms; defender specialties; healing of survivors; stability multipliers; sustaining/ending the storm; shared cap and pacing. These are [our mechanics](DESIGN.md), not existing Vintage Story canon. All agreed numbers and unresolved implementation cases remain as recorded there.
- **O2 — Approved visual direction:** veiled/recessed face, plated/cage head, bound/elongated arms, and an open U-shaped tuning fork for the bell specialist. [VISUALS.md](VISUALS.md) contains concept mockups; the images were viewed this pass. Placeholder names and the first three specialty assignments do not establish identities.
- **O3 — No new identity approved:** cultists, human survivors, independent otherworldly organisms, corrupted tools, and other possible explanations have not been selected. This pass does not propose one as fact. Coordination resembling ritual does not settle religion or intention.
- **O4 — Writing gate:** no finished collectible drafts, no new boss/finale, no loot decisions, and no gameplay implementation. Pass 2 below adds evidence and explicitly unselected interpretation proposals.

## Pass 2 — evidence added for integration proposals

The user requested actual lore-integration proposals while excluding the main characters from the four creatures' identities. The six remaining collectible groups were read in full, and `config/dialogue/resonator.json` was read with its referenced `dialogue-resonator-*` English entries. This completes reading the 101 piece references in the 19 collectible definitions, **not all game lore or environmental context**. See [coverage](LORE_SOURCE_COVERAGE.md).

Additional sources, all under the same installed English baseline:

| ID | Source / exact keys | Observation and limit |
|---|---|---|
| L16 | [resonator.json](E:/Games/Vintagestory/assets/survival/config/dialogue/resonator.json), components `engineer` through `engineer6`; EN `dialogue-resonator-confidentvoice`, `-understandable`, `-letmeclarify`, `-anincrediblefeat`, `-secondvoice`, `-firstvoice` | Recorded speakers explain the Chamber, bells, cylinders, and retrieval. Dialogue and conditions read; actual encounter/audio not tested. |
| L17 | EN `dialogue-wrigglelikeworms`, `dialogue-trulylostcreatures`, `dialogue-tremblewithit`; `trader.json` components `challenged`, `bonds`, `afraidso` | Remaining trader drifter responses continue a religious interpretation; no new objective origin proof. |
| L18 | [inmemoriam.json](E:/Games/Vintagestory/assets/survival/config/lore/inmemoriam.json); EN `lore-inmemoriam-piece1` | Multi-page, self-interested testimony concerning a supposed privileged refuge, deprivation, concealment, violence, and infection. Not a neutral account of every shelter. |
| L19 | [inconvenience.json](E:/Games/Vintagestory/assets/survival/config/lore/inconvenience.json); EN `lore-inconvenience-piece1` | Maintenance letter concerning generator, pump, gear, and access problems; specific mechanism context not independently verified. |
| L20 | [whinging.json](E:/Games/Vintagestory/assets/survival/config/lore/whinging.json); EN `lore-whinging-piece1` | A worker imagines mechanical help for repetitive fuel labor. A wish is not proof that the wished-for machine exists. |
| L21 | [miller.json](E:/Games/Vintagestory/assets/survival/config/lore/miller.json); EN `lore-miller-piece1` through `piece10` | Personal memory of ordinary life and separation during evacuation; useful for tone, not temporal technology. |
| L22 | [sparrow.json](E:/Games/Vintagestory/assets/survival/config/lore/sparrow.json); EN `lore-sparrow-piece1` through `piece10` | Correspondence and a final different voice portray the Flock, class conflict, and resistance. These characters/faction are not assigned to our quartet. |
| L23 | [discussions.json](E:/Games/Vintagestory/assets/survival/config/lore/discussions.json); EN `lore-discussions-piece1` through `piece4` | Dramatic presentation of moral arguments and hope; literary speakers do not give an objective cosmology. |

<details>
<summary><strong>Story/technology spoilers — the recorded engineer's explanation</strong></summary>

- **C16 — C-testimony:** a recorded engineer describes enduring sounds within the Chamber, bells sending sounds to it, and scribes transcribing retrieved material. EN `dialogue-resonator-confidentvoice`, `-understandable` (L16).
- **C17 — C-testimony:** the engineer explicitly distinguishes the Chamber as a receptacle from the bell that hears. EN `dialogue-resonator-understandable` (L16). Do not infer that the Chamber has awareness or commands creatures.
- **C18 — C-testimony:** cylinders are described as directions to sounds, rather than the sounds being physically inscribed on them; the resonator synchronizes to retrieve them. EN `dialogue-resonator-letmeclarify` (L16). This is a specific in-world technical explanation, not proof that a signal can materialize a body or sustain a rift.
- **I4 — Strong inference:** L8–L10 and L16 together make communication and recovery of lost information a particularly grounded theme for our mod. A forgotten local adaptation would still be our invention, not a newly discovered canonical system.
- **U9 — Proposal limit:** no reviewed text establishes that four voices can anchor a storm, that the Chamber creates beings, or that shivers/bowtorns were members of a historical rescue service. Any such connection must be marked original; avoid the latter two assertions in the recommended proposal.

</details>

**O5 — Proposed integration directions:** [LORE_DIRECTIONS.md](LORE_DIRECTIONS.md) compares remnants of a lost rescue apparatus (recommended), native beings maintaining a foothold, and a recurring impression of a rescue party. None selected. All exclude identities drawn from the main cast. Proposed explanations support existing mechanics without finalizing specialist assignments or lifecycle edge cases.

## Continuation task

**2026-10-25 update:** the user accepted Direction A and requested six collectible drafts. [LORE_COLLECTIBLES.md](LORE_COLLECTIBLES.md) records all six, original authors and circumstances, suggested placements, knowledge boundaries, and exact source keys used as prose references. The lost rescue operation is now the accepted mod premise; specific wording and supporting details remain drafts. Earlier statements of unselected status above describe passes 1–2, not the current decision.

Review the drafts with the user. Further evidence work should prioritize remaining dialogue, environmental/artwork context, and statements capable of challenging C5–C18. The rescue/recording connection has explicit dialogue support; the four-part apparatus and storm effects remain original additions. Preserve low-spoiler summaries and separate major revelations.

**Further 2026-10-25 writing request:** two original Archives-only records (P7–P8 in [LORE_COLLECTIBLES.md](LORE_COLLECTIBLES.md)) add the four's subjective reactions to entrapment and slow mental deterioration. `lore-breakdown-piece1`, `lore-return-piece1`, `dialogue-resonator-confidentvoice`, and `dialogue-resonator-understandable` were re-read as prose/provenance references. No new canon was inferred from them. The records' preserved voices, anonymous transcription, and lost-worksite provenance are original additions; exact source identities and the nature of their memories remain unresolved. Eight total drafts now exist.
