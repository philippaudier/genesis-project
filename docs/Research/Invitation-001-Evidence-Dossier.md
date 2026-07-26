# Invitation-001 — Evidence Dossier

*Prepared for the second researcher of Genesis-019. This document contains facts only:
the rules of the system, the experimental conventions, and the measured record. It deliberately
contains no explanation, no interpretive vocabulary, and no hypothesis. It is committed before the
invitation so that its exact content is auditable.*

---

## 1. The system

A deterministic discrete-time simulation. State: a set of **places**; each place holds one signed
64-bit integer quantity (and optionally a second integer, a *rate*). Places are connected by
directed **relations**; every connection below is symmetric (both directions present).

Each tick, every place applies the same two rules, reading a frozen snapshot of the previous
state; all changes are summed and committed simultaneously:

- **Transfer rule.** For each outgoing relation from place `s` to neighbour `n`: if
  `value(s) > value(n)`, transfer `floor((value(s) − value(n)) / 2)` units from `s` to `n` — but
  only if that amount is > 0. (A difference of 1 transfers nothing.)
- **Production rule.** If a place has rate r > 0, add r to its quantity each tick. Worlds without
  rates are called **closed**; with rates, **sourced**.

There is no randomness. Same initial state ⇒ same run, always. Quantities may go negative; nothing
forbids it. Integer arithmetic wraps at 64 bits.

## 2. Conventions used in the record

- **Freeze tick**: first tick whose state equals the previous tick's state. (One earlier record
  used last-change tick; where relevant this is flagged.)
- **Firing edge** at tick t: a directed relation that actually transfers a nonzero amount at t.
- **F(t)**: the set of firing edges at tick t (logged in recent experiments).
- **Ratio**: value(t)/value(t−1) at the place with the largest |value|.
- Claim domain used recently: |values| < 2⁴⁰.

## 3. The measured record

### 3.1 Small closed worlds

- **Ring of 3** (3 places in a cycle), load 12 on one place: values slosh, then reach (4,4,4) and
  freeze. Total is 12 at every tick.
- **Star, centre + 3 leaves**, centre loaded 12: from (centre 12; leaves 0,0,0) the state goes to
  (centre −6; leaves 6,6,6) and back, exactly, forever — period 2. The centre is negative on
  alternate ticks. Total 12 always.
- **Same star, load 12 on a LEAF instead**: (12,0,0 leaves; 0 centre) → (6,0,0;6) → (6,3,3;0) →
  (3,2,2;5) → (4,3,3;2) → (3,3,3;3) frozen. **No value was ever negative.** Freeze at t=6
  (t=7 by the first-repeat convention).
- **Star, centre + 4 leaves**, centre 12: values grow without bound, sign of the centre
  alternating each tick; measured asymptotic |ratio| → 1.5 exactly.
- **Star, centre + 5 leaves**, centre 12: same shape of behaviour; asymptotic |ratio| → 2 exactly.
- **Complete graph on 4 places**, one place loaded 12: period-2 oscillation.
- **A tree whose maximum degree is 3** (structure: branching tree, no cycle): values grew without
  bound. (Recorded fact: unbounded growth occurred in a max-degree-3 tree.)

### 3.2 The centre-loaded 3-star, load series (exact measurements)

| load | outcome | freeze/period tick (first-repeat) | centre's most negative value |
|---|---|---|---|
| 12 | period 2 | — | −6 |
| 13 | frozen | 7 | −5 |
| 15 | frozen | 8 | −6 |
| 120 | period 2 | — | −60 |
| 121 | frozen | 61 | −59 |
| 1200 | period 2 | — | −600 |
| 1201 | frozen | 601 | −599 |
| 12001 | frozen | 6001 | −5999 |

### 3.3 Random-world survey

1,200 random closed worlds (3–7 places, random symmetric edges, one or two loads in 6–24): during
their runs, negative values occurred and healed in 21; occurred and persisted periodically in 55;
413 worlds grew without bound. **Every periodic world ever seen — here and elsewhere — has period
exactly 2. No other period has ever been observed.** 800 random sourced worlds: only 36 ever showed
a negative value.

Sourced worlds grow steadily; in two different topologies, the growing internal shape repeated
with period 3 (the values grow, but the *pattern* of which places are ahead repeats every 3 ticks).

In several worlds, some relations **never** carried a single unit across the entire run, despite
being present in the topology.

### 3.4 The 64-bit frontier

The 5-leaf star (centre 12) was deliberately run past 64-bit overflow. The displayed total stayed
exactly 12 at every tick, through wraparound. Around t≈63 the world reached the value
4,611,686,018,427,387,906 (≈2⁶²) at the centre and froze there permanently.

### 3.5 The double star (9 places) — exact trajectory

Two stars, each a centre with 4 leaves, sharing exactly one common leaf (9 places total: centres
A and B; A's private leaves ×3; B's private leaves ×3; one shared leaf S). Load 12 at centre A.

| t | A | B | S |
|---|---|---|---|
| 1 | −12 | 0 | 6 |
| 2 | 24 | 3 | −6 |
| 3 | −30 | −4 | 13 |
| 4 | 51 | 10 | −16 |
| 5 | −72 | −18 | 30 |
| 6 | 117 | 39 | −45 |
| 8 | 283 | 135 | −124 |
| 10 | 708 | 420 | −339 |
| 15 | −7648 | −6129 | 4174 |
| 20 | 88623 | 80577 | −51228 |

|ratio| at successive ticks: 1.000, 2.000, 1.250, 1.700, 1.412, 1.625, 1.513, 1.599, 1.562,
1.602, 1.590, 1.609, 1.610, 1.618, 1.621 (t=1..15) … converging to **1.65137** by t=60. Total 12
at every tick, through amplitudes of 4×10¹³.

**Same world, load 49152 (=12×4096) at A**: |ratio| series t=1..15: 1.000, 2.000, 1.3125, 1.7143,
1.4653, 1.6303, 1.5407, 1.6094, 1.5798, 1.6099, 1.6022, 1.6166, 1.6165, 1.6242, 1.6262. The first
two ticks match the load-12 run to every digit; later ticks differ slightly.

### 3.6 The chain of 5 — trajectory with firing log

Places 0–1–2–3–4 in a line, load 16 at place 0:

| t | values | F(t) |
|---|---|---|
| 1 | 8 8 0 0 0 | {1→2} |
| 2 | 8 4 4 0 0 | {0→1, 2→3} |
| 3 | 6 6 2 2 0 | {1→2, 3→4} |
| 4 | 6 4 4 1 1 | {0→1, 2→3} |
| 5 | 5 5 3 2 1 | {1→2} |
| 6 | 5 4 4 2 1 | {2→3} |
| 7 | 5 4 3 3 1 | {3→4} |
| 8 | 5 4 3 2 2 | {} |
| 9 | 5 4 3 2 2 | frozen |

### 3.7 The bridged graph (8 places) — trajectory with firing log

A 4-leaf star (centre a, leaves l1..l4) joined to a 3-neighbour hub (centre b, neighbours m1, m2,
and l4) by the edge l4–b. Load 12 at a.

| t | a | l1 | l2 | l3 | l4 | b | m1 | m2 |
|---|---|---|---|---|---|---|---|---|
| 1 | −12 | 6 | 6 | 6 | 6 | 0 | 0 | 0 |
| 2 | 24 | −3 | −3 | −3 | −6 | 3 | 0 | 0 |
| 3 | −30 | 10 | 10 | 10 | 13 | −3 | 1 | 1 |
| 4 | 51 | −10 | −10 | −10 | −16 | 9 | −1 | −1 |
| 5 | −72 | 20 | 20 | 20 | 29 | −13 | 4 | 4 |
| 6 | 116 | −26 | −26 | −26 | −42 | 24 | −4 | −4 |

F(t) alternates between exactly two sets from t=2 onward, with no exception through t=58 (where
|values| passed 2⁴⁰): `{a→each leaf, b→l4, b→m1, b→m2}` on even ticks and
`{each leaf→a, l4→b, m1→b, m2→b}` on odd ticks. Four distinct F-sets were seen in the entire run
(the initial one, one transient, and the two alternating ones). Asymptotic |ratio| measured over
t=54..58: **1.56244**, stable to 5 decimal places. Total 12 at every tick.

### 3.8 The 3-chain pair (gradient detail)

- Chain 0–1–2 loaded (2,1,0): **no value ever changed** through t=100.
- Chain 0–1–2 loaded (3,1,0): → (2,2,0) → (2,1,1), frozen (final largest neighbour-difference: 1).

### 3.9 Frozen states, collected

Every frozen final state on record: (4,4,4) · (3,3,3;3) · (5,4,3) [an older 3-place case] ·
(5,4,3,2,2) · (2,1,0) · (2,1,1) · the near-equal star states of §3.2. In every one of them, every
pair of connected places differs by at most 1. The one exception on record is the ≈2⁶² freeze of
§3.4, which occurred beyond the 64-bit wrap.

---

*End of dossier. Everything above is measurement or system definition. Anything that sounds like
an explanation is an accident of prose and carries no authority.*
