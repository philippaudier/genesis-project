# RFC-0002 — Architectural Review

Status: **Review — decision-space completeness.** Reviews `RFC-0002-Simulation-Transformation-Pipeline.md`
as a skeptical senior architect. Does not rewrite it, continue it, or choose an option. The single
question: **is the decision space complete enough to make a five-year, hard-to-reverse choice?**

> Verdict: **No — not yet.** The RFC is disciplined (it refuses to choose, folds in the research arc,
> carries kill criteria and a dependency graph, and correctly separates constraints from
> transformation). But three **Critical** gaps — each contradicting one of Genesis's own prior
> artifacts — mean that accepting this RFC as the decision *frame* would risk an irreversible choice
> made on a truncated space. None of the three require choosing an architecture to fix; each requires
> *widening the space*. The Majors should be fixed but do not individually block.

Each finding: **Severity · Why it matters · Blocks the decision? · Smallest change.**

---

## Critical

### C-1 — The candidate space silently assumes all transformation is per-tick and uniform, contradicting RFC-0001

**Why it matters.** Every candidate family (Pipeline, Systems, Phases, Rule Graph) is an answer to
"what runs *each tick*." Nowhere does the scheduling axis admit a transformation *dated to a future
tick* — one that is not evaluated every tick but fires once at tick T. Yet RFC-0001's own preliminary
direction was **Option 4: fixed tick plus scheduled events**, precisely to avoid recomputing sparse,
long-horizon change every tick. RFC-0002 *depends on* RFC-0001 and then quietly drops the dimension
RFC-0001 leaned toward. A five-year architecture that can only express "every tick, everything" will
force every sparse or long-dormant phenomenon (an eruption at tick 10⁶, a seasonal turn, an egg that
hatches later) into a per-tick unit that mostly no-ops — the exact inefficiency and inexpressiveness
RFC-0001 flagged.

**Blocks the decision?** **Yes.** The temporal scope of transformation is a first-order structural
choice; deciding the unit/scheduler pair without it bakes in "uniform per-tick" by omission.

**Smallest change.** Add a **temporal-scope dimension** to the decision (per-tick / event-scheduled /
continuous), and one line reconciling it with RFC-0001 Option 4. No new prose about *which* to pick.

### C-2 — The RFC assumes a single unified transformation mechanism, contradicting the research's own W1 finding; and the CA / local-field family is missing

**Why it matters.** The framing "a clean decision likely chooses *one* unit abstraction and *one*
scheduling abstraction" quietly decides against **pluralism** — different mechanisms for different
kinds of change. But the research arc's most durable unresolved finding (W1, `Minimal-Architecture-
Critique.md`) was that **continuous conserved fields and discrete agents do not fit one mechanism**:
Intent/System/Command shapes fit agent-driven change and fail continuous fields. The RFC even names
the **scalar-diffusion experiment** as validation — yet diffusion is a *uniform local update over all
cells*, which is not cleanly any of the six candidates. When the RFC's own validation example doesn't
fit its candidate set, the set is incomplete, and the "choose one pair" frame has pre-decided a
question (unified vs plural) that the research left open.

**Blocks the decision?** **Yes.** "One mechanism or several?" is logically prior to "which one?" and is
currently answered by omission.

**Smallest change.** Add an explicit **meta-question** ("Is the answer a single mechanism, or a
plurality keyed to phenomenon type?") to Open Questions, and add the missing **cellular-automaton /
uniform-local-update** family to Options Considered. No verdict required.

### C-3 — The evaluation omits emergence — the project's defining criterion

**Why it matters.** The requirements list C5 (Article VIII: complexity emerges from the *interaction*
of simple units). The comparison matrix then scores seven axes — determinism, tests, replay, saves,
performance, readability, extensibility — and **none of them measures interaction or emergence-
enablement.** For a framework whose entire purpose (Constitution, Vision) is emergent worlds, judging
transformation mechanisms without asking "which best enables rich interaction between units?" is
optimising everything except the goal. A strict pipeline (units interacting only through end-to-end
state flow) and a shared-state model with declared reads (units genuinely composing) score identically
on the current axes yet differ enormously on emergence. The mechanism shape *does* bound what
interactions are possible; leaving it unmeasured invites a technically clean architecture that
produces dead worlds.

**Blocks the decision?** **Yes.** A decision optimised on the current matrix could be actively wrong
for Genesis's stated purpose.

**Smallest change.** Add one evaluation axis — **interaction / emergence-enablement** — to the matrix
and the requirements-to-axis mapping. (Nuance to note, not resolve: emergence also depends on content,
not only mechanism.)

---

## Major

### M-1 — The "two axes" decomposition is presented as complete but omits a third

**Severity: Major.** The RFC's headline insight — unit axis × scheduling axis — is genuinely valuable,
but it is asserted as *the* decomposition ("two questions wearing one coat"), while the **state-access
and mutation model** (in-place vs new-state; declared/scoped/ambient reads) is demoted to open
questions Q2/Q5. That third axis is arguably the most consequential: it governs saves, replay,
parallelism, and the feel of the whole system. **Blocks?** No, but it distorts the framing.
**Smallest change:** promote mutation/state-access to a named third axis alongside unit and scheduling.

### M-2 — Q2 (mutation) and Q3 (snapshot vs sequential) are listed as independent but co-determine each other

**Severity: Major.** Snapshot evaluation essentially requires new-state / double-buffering; sequential
evaluation presumes in-place mutation. Choosing one largely forces the other. Listing them as separate
open questions understates that they are one coupled decision. **Blocks?** No. **Smallest change:** a
sentence noting Q2 and Q3 are jointly constrained, resolved together.

### M-3 — Several evaluation criteria are collinear, inflating apparent dimensionality

**Severity: Major.** Replay *requires* determinism; saves feed checkpoint-replay; testability is
easiest under determinism/purity. So a family's determinism rating largely predicts its replay, saves,
and testability ratings — the matrix counts one property (purity/determinism) up to four times, which
mechanically over-rewards functional families and over-penalises others. **Blocks?** Borderline — it
biases the decision. **Smallest change:** mark the dependent axes (or collapse determinism/replay/saves
into one), so the matrix weights *independent* factors.

### M-4 — "Unit" and "scheduling" each conflate distinct sub-choices

**Severity: Major.** "Unit" mixes *granularity* (how much a unit does — a System is coarse, a Command
is an instance) with *form* (function vs data vs object). "Scheduling" bundles *total order* (pipeline),
*grouping* (phases), and *partial order* (graph) as if they were one choice. Comparing families across
conflated sub-axes produces apples-to-oranges rows. **Blocks?** No. **Smallest change:** split each
axis into its sub-choices in the terminology/framing.

### M-5 — Monolithic-state assumption sits in tension with Locality (Article VII)

**Severity: Major.** Units are described as reading/writing "the state" (one object, inherited from
Genesis-003's single `SimulationState`). Article VII wants units acting on bounded neighbourhoods, and
future spatial/partitioned state is likely. The RFC treats access as Q5 but never questions whether
"the state" is one thing — so partitioned-state and locality-first models are silently disadvantaged.
**Blocks?** No. **Smallest change:** note "is the state monolithic or partitioned?" as an open question
feeding the access axis.

### M-6 — No failure / partiality model

**Severity: Major.** Over five years, transformations *will* be unable to apply (precondition unmet,
resource absent, conflict). The RFC covers the *constraint seam* (external rejection) but not a unit's
own **failure semantics** — throw? no-op? produce nothing? partial application? This shapes determinism
and composition and is absent from the dimensions. **Blocks?** No, but it will resurface as rework.
**Smallest change:** add "unit failure/partiality semantics" to Open Questions.

---

## Minor

- **Performance is scored as an axis while declared a non-priority.** Including it invites the decision
  to be swayed by a factor the project says it doesn't yet weight. *Fix:* mark it non-weighted-for-now.
- **In-place-mutation language leaks a lean.** Phrases like "the state it changes" presume mutation
  even though Q2 is "open," mildly pre-loading the answer. *Fix:* neutral phrasing ("the state it
  transforms").
- **A data-driven / declarative-rules family is absent** (rules as interpreted data, distinct from
  code units) — the research even noted rule-content can be state. *Fix:* name it as a family or
  explicitly exclude it.
- **Rule Graph silently assumes a static graph** (topological sort); state-dependent dynamic
  dependencies are excluded without comment. *Fix:* one clause.
- **Validation-Plan vs the distilled methodology:** ensure the diffusion experiment is framed as
  *informing* the choice, not *making* it (Principle III: experiments eliminate, they do not decide).
  The RFC mostly respects this ("where analysis is insufficient"); keep it explicit.

---

## What is genuinely strong (not padding)

- **It refuses to choose.** Correct, and rare — the comparison is kept separate from the commitment.
- **The unit-vs-scheduling disentanglement is a real contribution**, even though it is incomplete
  (M-1/M-4). It already dissolves false rivalries (Phases order Systems; Pipelines carry Transitions).
- **Constraints are correctly held apart from transformation**, with a required seam and a deferral to
  their own RFC — this prevents the "validation becomes an unbounded solver" failure the research found.
- **It folds in the research arc as prior deliberation** rather than re-deriving, and reports the prior
  leaning as *input* with Decision: Pending — exactly the discipline intended.
- **Kill Criteria and Dependencies are present and honest.**

None of these strengths offset the Criticals, because the strengths are about *how well the space is
reasoned over*, and the Criticals are about *the space being too small*.

---

## Bottom line

The RFC would make a good decision *frame* if the space were complete. It is not. Three dimensions are
missing, and each is missing in a way that has already **silently pre-decided** a question the project
left open elsewhere:

1. **Temporal scope** — pre-decided "uniform per-tick," against RFC-0001 Option 4. (C-1)
2. **One mechanism vs several** — pre-decided "one," against the research's W1 finding; the field/CA
   family is absent. (C-2)
3. **Emergence** — absent from the criteria, against the project's defining purpose. (C-3)

**Recommendation:** RFC-0002 should not advance from comparison to *selection* until C-1, C-2, and C-3
are added to the decision space. Critically, **fixing all three requires only widening the space —
adding a dimension, a meta-question, a candidate family, and an evaluation axis — not choosing any
architecture.** The Majors (especially M-1 the third axis, and M-3 the collinear criteria) should be
folded in next, as they distort *how* the eventual choice is weighed. Do this and the RFC becomes what
it intends to be: a complete map of an irreversible fork, chosen with eyes open.
