# 2026-07-26 — The kernel, told as six questions

Genesis-004 through Genesis-009 are complete: 45 EditMode tests, all green. This entry freezes the
story they tell, because the sequence turned out to be the point.

Each milestone answered exactly one fundamental question, and none answered more:

- **Genesis-004 — How does time advance?** A tick reads an immutable snapshot and produces the next
  state. Two runs from the same beginning end strictly identical.
- **Genesis-005 — How do computations compose?** Independent transitions combine in one tick without
  knowing each other, and their order cannot matter.
- **Genesis-006 — How are conflicts resolved?** Writes are identifiable contributions; competing
  contributions to the same target resolve exactly once, by an explicit commutative rule, or are
  rejected. Order-independence disqualified last-writer-wins before taste ever entered it.
- **Genesis-007 — What may a cause observe?** Reads became as explicit as writes. A transition
  declares its scope and receives a view in which undeclared state is not hidden but absent.
- **Genesis-008 — What possesses identity?** State locations acquired stable, explicit addresses.
  Kind and identity were separated; equality became a property of address/value pairs, never of
  storage order.
- **Genesis-009 — How are things related?** Addresses can be connected by explicit, directed,
  meaningless relations — validated against what exists, changing nothing, deciding nothing about
  space.

Notable in hindsight: not one of these milestones mentions a creature, a terrain, an item, or a
player. The engine has laws before it has a world. Most engines begin with objects — Entity,
GameObject, Actor — and grow rules around them; Genesis inverted this and built the laws first. The
world, when it arrives, will have to obey physics that already existed before it.

The other thing worth recording is the method, because it held for six consecutive milestones: define
the proof sentence first ("Genesis-N proved that…"), build the smallest witness that could make it
true, keep every earlier proof running as a regression guard, and stop when done. The counters —
which have now witnessed snapshots, composition, conflict, scoping, identity, and relation — never
once mattered in themselves. They were always the stand-in for a world that does not exist yet.

What comes next is not another law. Genesis-010 will ask what a transition may observe *through* a
relation, and Genesis-011 is expected to be the first phenomenon — behaviour that could not exist
without relations. That is where emergence formally begins, and where the kernel stops being tested
against witnesses and starts being tested against the thing it was built for.

The kernel is frozen here. 45 proofs, six questions, no world yet — on purpose.
