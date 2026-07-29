# Demo-001 — A world to look at

> **This demonstration belongs to no campaign.** Nothing seen here is evidence, may be cited as
> evidence, or may be scored against any prediction. No claim is sealed, no observation is
> filed, no agenda entry is created, no reduction is performed. Looking at a world is not an
> experiment; filing a claim about one is.

## Declarations (recorded so nothing can be laundered later)

1. **Non-evidential.** Whatever this world does, it says nothing about H-S1-1, F-1, CT-003, or
   any open question. It is a picture, not a measurement.
2. **Parameters were chosen for visibility.** Deliberately — that is what a demonstration is,
   and it is exactly why nothing here counts. A campaign chooses parameters before knowing the
   answer; a demo chooses them to be worth looking at.
3. **Prior-knowledge declaration.** From this date, the executor has watched water move on a
   grid. Any future campaign touching grid flow must list this demo in its Blind Spot Audit as
   prior exposure. (The Run-000003 precedent: declare the contamination, never hide it.)
4. **No law enters the corpus.** The fixtures are S1-001's, **reused unchanged** — Fixture
   Transparency exercised a second time. Nothing was added to `Assets/Genesis/Simulation`.
5. **One demonstration choice, declared:** a **constant divisor of 5**. It coincides with the
   degree-aware policy on interior cells (degree 4 → 5) and removes a border artifact where
   corners (degree 2 → 3) would transport on dry ground. It is a demo convention, not a
   studied law.

## The one thing that *was* derived by hand

The terrain is built **using** what S1-002 confirmed, instead of hoping:

> An edge transports iff the potential difference reaches the local divisor.

So the terrain is constructed with **every elevation step ≤ 4**, strictly below the divisor 5.
Consequence, known before running: **dry ground is inert** — exactly Obs-011's stasis, on
purpose. Nothing moves because of the terrain. Water moves only where *water itself* has raised
the potential difference to 5.

That is the whole design: the landscape holds still, and the water is the only thing alive in
it. The builder verifies the max gradient at construction and refuses to run if it exceeds 4.

## The world

- 32 × 32 grid, 4 neighbours, constant divisor 5.
- Elevation: a slope (3 per row) carrying three shallow waves across its width, fading to a
  flat basin over the last 6 rows. Range ≈ 0–84. No noise, no randomness — a closed formula.
- Rain: +1 into water at every cell of the top row, boundaries 0–299, then the sky closes.
- 400 ticks: ~300 of rain, ~100 of the world settling with nothing added.

## Running it

```
cd Lab/Demo-001
dotnet run
```

Writes `Record/demo-001.record` (git-ignored — a demo artifact is not a proof).
Then open Unity, put `TerrainRecordPlayer` on an empty GameObject, press Play.

## What you are looking at

The rendered height is **elevation + water** — the potential the law actually reads. Colour:
dry ground shaded by slope, water by depth, and **negative water in red**, because positivity
is not a theorem here (Obs-004) and an instrument must never hide what it knows.

The headless summary also reports the greatest water-depth range across the columns of any row,
over the entire run. This is a **demo diagnostic, not a measurement**: it prevents the visible
troughs from being mistaken for drainage channels if the water actually remains uniform across
them.

## Routing probe

`dotnet run -- --routing-probe` runs a deliberately non-evidential A/B pair: the same 7×7 slope
and the same point rain, once laterally flat and once with a sub-threshold corrugation. It reports
only the first tick at which their water states differ. The exact pair is **prior exposure** and
can never become a campaign specimen; if it reveals a viable question, a future sealed campaign
must use independent parcels and declare this probe in its Blind Spot Audit.
