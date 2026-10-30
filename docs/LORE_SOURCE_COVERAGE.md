# Lore source coverage — passes 1–2

Snapshot researched 2026-10-24, installed client reported 1.22.7 Stable. This is a bounded audit, **not all Vintage Story lore**. Findings and source IDs are in [LORE_RESEARCH.md](LORE_RESEARCH.md).

## Installed collectible definitions

Directory: `E:\Games\Vintagestory\assets\survival\config\lore`.

All **19 definition files** were enumerated and their localization references checked against `assets/game/lang/en.json`: **101 piece references**, all present. A piece can contain several pages; this is not 101 books. Reviewed means the named English entries were read in full, not that world placement, artwork, audio, authorship, or runtime availability was verified. Partial means an excerpt was encountered but no complete review is claimed.

| Definition | English title | Piece references | Text coverage |
|---|---|---:|---|
| `blindbatrickhart.json` | Blind Bat Rickhart | 8 | Reviewed; L13 |
| `breakdown.json` | Breakdown | 1 | Reviewed; L3 |
| `confession.json` | Confession | 1 | Reviewed; L4 |
| `dietofkings.json` | Diet of Kings | 1 | Reviewed; L6 |
| `discussions.json` | Brief Discussions with the Traveler | 4 | Reviewed in pass 2; L23 |
| `dmetrisnotes.json` | Dmetri's Notes | 5 | Reviewed; L5 |
| `inconvenience.json` | An Inconvenience | 1 | Text reviewed in pass 2; L19; encounter context pending |
| `inmemoriam.json` | In Memoriam | 1 | Reviewed in pass 2; L18 |
| `letters.json` | Letters | 8 | Reviewed; L7 |
| `miller.json` | Admirer of the Miller | 10 | Reviewed in pass 2; L21 |
| `patronage.json` | The Patronage of Tibalt Amaro | 5 | Reviewed; L10 |
| `reflection.json` | Reflection | 5 | Reviewed; L2 |
| `return.json` | Return | 2 | Reviewed; L11 |
| `sparrow.json` | The Spy and the Sparrow | 10 | Reviewed in pass 2; L22 |
| `stone.json` | The Weight of Stone | 9 | Reviewed; L9 |
| `tapestry.json` | Tapestry | 18 | Text reviewed; L8; images/context pending |
| `themorning.json` | The Morning | 5 | Reviewed; L12 |
| `wallcarving.json` | Wall Carving | 6 | Text reviewed; L14; placement pending |
| `whinging.json` | Whinging | 1 | Reviewed in pass 2; L20 |

Pass 1 reviewed 13 groups / 74 referenced pieces. Pass 2 completed the remaining six groups / 27 references: **19 groups / 101 references now read**. This counts localization entries only and does not claim all game narrative is in this directory. Text-heavy output was sometimes truncated in pass 1; the remaining groups were retrieved in bounded, untruncated pass-2 reads. Environmental and encounter context remain pending.

## Handbook, dialogue, and other local evidence

- Read `config/handbook/13-temporalstability.json` and both referenced English entries in full (L1).
- Enumerated **25 dialogue files** under `config/dialogue`, including `example.json`. This count is files, not necessarily 25 characters or encounters.
- `trader.json`: inspected opening components and drifter conversation structure; resolved `dialogue-miserablesouls`, adjacent player choices, and in pass 2 the three remaining drifter branch responses (L17). Full conversation not reviewed. Player options must not be quoted as the trader's beliefs.
- `resonator.json`: full definition and English `dialogue-resonator-*` entries read in pass 2, including engineer and alchemist branches. Entry conditions inspected; live reachability and recorded audio not tested. Ledger details concentrate on the engineer's explanation (L16); the alchemist's personal storyline is not adopted for the mod.
- Remaining dialogue files are **inventoried only**: `agnieszka.json`, `alba.json`, `arzhur.json`, `beata.json`, `example.json`, `folds.json`, `gerhardt.json`, `indira.json`, `kat.json`, `laura.json`, `lena.json`, `liga.json`, `norbert.json`, `piotr.json`, `sedna.json`, `skeletonarm.json`, `skeletonwithloot.json`, `tad.json`, `tobias.json`, `treasurehunter.json`, `villager.json`, `waclaw.json`, `wall.json`.
- Partially read `entities/lore/bell.json` and `entities/lore/locust.json`: headers/mechanical flags/variants and bell reinforcement task (A1–A2). Prior-session entity balance research remains separately identified in [RESEARCH.md](RESEARCH.md).
- Read English item keys `itemdesc-gear-temporal` and `itemdesc-spear-generic-hacking` (A3). No exhaustive item/block description review.
- Viewed both project concept images listed in [VISUALS.md](VISUALS.md); these are mod design references, not game lore evidence.
- Did not inspect game paintings/tapestry images, models, environmental arrangement, story schematics, dialogue triggers in a live encounter, audio/recording contents, or effective mod-patched assets.

## Web coverage and failures

- Read official overview and full official excerpts The Morning and Ghosts (W1–W3). No claim that these are an exhaustive official fiction corpus. The local Morning text was also read; no exhaustive word-level equivalence comparison.
- Read wiki Temporal stability, Temporal storm, Temporal rift (W4–W6), used as supporting indices. Record their rendered revision IDs in the ledger. Exact revision dates/target versions not verified; linked 1.16 release notes not yet followed.
- Rust World wiki link and capitalization variants failed to fetch. Page-info date queries and attempts to reopen pinned oldid URLs also failed. These are access gaps, not evidence that the underlying topic has no page or no lore.
- Broad search snippets are discovery leads only. No community post, fan timeline, video/transcript, or unattributed lore summary was adopted as canon.
- Exa via agent-reach failed DNS resolution. Built-in web access worked; no software installation/configuration change was made.

## Remaining research priorities after pass 2

1. Direction A was selected on 2026-10-25; review [eight original collectible drafts](LORE_COLLECTIBLES.md). All exclude the main cast as creature identities or required participants. This drafting update does not add claims of further corpus coverage.
2. Trace remaining relevant dialogue through localized strings and conditions, prioritizing evidence that could challenge the proposed technological/historical connections. Record spoilers separately.
3. Map story-location artifacts, tapestry images, and machine encounters to the texts. Identify which witnesses could know which events.
4. Establish what the actual evidence says about shivers/bowtorns/drifters, versus merely sharing their spawn pools.
5. Inspect versioned official developer statements and wiki citations about rifts/storms and relevant story content. Resolve mechanical conflicts only if needed for lore; keep untested runtime claims explicit.
6. Compare key terminology across languages only where ambiguity matters. Do not invent a translation dispute without checking the texts.
