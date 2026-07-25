# Genesis Constitution

*The foundational principles of Genesis. This document defines what Genesis is, independent of any language, library, or machine. It is expected to outlive every implementation built beneath it.*

---

## Preamble

Most simulated worlds are built backwards. They begin with a story the author wants to tell, or an experience the author wants a player to have, and then they construct the minimum machinery required to sustain that illusion. The world is a stage. Its depth ends precisely where the audience stops looking. Behind the visible surface there is no country, only the painted flat that suggests one.

This produces worlds that are convincing for as long as they are observed in the intended way, and hollow the moment they are not. Walk to the edge of the map and the world ends. Return to a village a hundred days later and it is exactly as you left it, frozen in the instant of your departure, waiting. Nothing happened while you were gone because, in truth, there was nothing there to happen to. The world was never alive. It was a response to attention.

Genesis begins from the opposite conviction: that a world worth believing in must exist whether or not anyone is watching. It must have causes and consequences that do not depend on an observer to occur. It must be able to surprise the very people who built it. A world like this cannot be authored scene by scene. It can only be *grown* — cultivated from a small set of rules simple enough to be understood completely, and rich enough that their interactions exceed anyone's ability to predict them.

Genesis is an experiment in that kind of cultivation. It is a framework for exploring how believable worlds emerge from a small number of simple, deterministic rules. It does not aim to script behavior. It aims to create the conditions from which behavior arises on its own.

This document is the constitution of that experiment. It is not a specification, an architecture, or a design. It is the layer beneath all of those — the set of commitments from which the specifications, architectures, and designs are meant to descend. Where a future decision conflicts with what is written here, the decision is what should be reconsidered. The Constitution changes only when we discover that a principle was wrong, never when it is merely inconvenient.

---

## Mission

**Genesis exists to discover how much believable world can emerge from how little rule.**

We build a deterministic simulation that runs on its own terms, and we study what it produces. The simulation is the work. Everything else — every window we open onto it, every way we choose to render or narrate or play within it — is an interface to something that would continue without us.

---

## Vision

We imagine a world that a person could return to after a long absence and find genuinely changed — not because a designer authored the change, but because the world lived through the interval. Rivers will have shifted their courses. Settlements will have risen where trade made them inevitable and emptied where it did not. Grievances will have hardened into feuds or dissolved into alliances, for reasons that can be traced, step by step, back through the causes that produced them.

We imagine no scene in this world having been written, and every scene being explicable. We imagine the people who built Genesis studying its histories the way naturalists study an ecosystem: not as authors reviewing their own text, but as observers of something that has taken on a life of its own.

If Genesis succeeds, its most compelling moments will be ones no one designed. That is the entire ambition.

---

## Articles of the Constitution

### Article I — The World

**The world is the only source of truth.**

There is exactly one authoritative account of what exists and what is happening, and it is the state of the simulated world itself. Not a script describing what should happen. Not a narrative layer interpreting events for an audience. Not the contents of any single view onto the world. The world is not a representation of some truer thing elsewhere. It *is* the thing.

This principle exists because a world with two masters has none. The moment there is an authoritative script running alongside the simulation — telling a character to be in a certain place, feel a certain way, want a certain outcome regardless of what the simulation would produce — the simulation ceases to be believable and becomes merely decorated. The seams between what is simulated and what is dictated are where belief dies.

The consequence for all future development is a single, demanding question applied to every piece of state: *does this live in the world, or does it live somewhere else pretending the world agrees?* Anything that describes, predicts, or narrates the world must derive from the world and hold no authority over it. When a view and the world disagree, the world is right and the view is stale. There is no appeal above the world, because there is nothing above it.

### Article II — The Displaced Observer

**The player is not the center of the world.**

Whatever agent observes or participates in Genesis — a player, a researcher, an automated probe, no one at all — is a participant in the world, not its purpose. The world does not orient itself around the observer. It does not spawn its events in response to their approach or suspend them in their absence. It does not scale its challenges to their capability or arrange its coincidences for their benefit. The observer enters a world already in motion and leaves it still in motion.

This is the hardest principle to honor, because nearly every convention of interactive worlds violates it. Difficulty that adjusts to the participant. Events that trigger on proximity. Characters who exist only to be encountered. Each is a small confession that the world is really about the one watching it. Genesis refuses these not out of austerity but because each one, however small, reintroduces the second master that Article I forbids.

The consequence is that Genesis must be designed as though no one will ever observe it, and then made observable as a separate concern. A world that only makes sense when someone is looking at it in the intended way is not a world. It is a performance. Genesis is not a performance.

### Article III — Independence of Observation

**The simulation exists independently of observation.**

The world advances by its own logic, at its own pace, regardless of whether any part of it is currently being observed. A region no one is looking at is not paused, not approximated into nonexistence, not resumed from a frozen snapshot when attention returns. It has been living the entire time. What an observer finds when they arrive is the honest consequence of everything that happened while they were elsewhere.

This is the principle most often sacrificed for expedience, and its sacrifice is always visible in the end. Worlds that only simulate what is watched are worlds where nothing has a past. Their inhabitants have no memory of the unobserved interval because, for them, no interval occurred. Genesis treats the unobserved world as fully real, because a history that only exists where someone was looking is not a history at all.

We acknowledge plainly that a truly unbounded world cannot be computed in full forever. Independence of observation is therefore also a discipline of *honest approximation*: where the world must be summarized to remain tractable, it is summarized by rules that themselves belong to the world and preserve its causes — never by simply ceasing to exist and later inventing a plausible present. The distinction is fundamental. One is compression of a real history. The other is fabrication of a fake one. Genesis compresses. It never fabricates.

### Article IV — Determinism

**The same beginning produces the same world.**

Given identical initial conditions and identical inputs, Genesis produces identical outcomes, every time, everywhere, forever. The world's evolution is a function of its causes and nothing else. Randomness, where it appears, is drawn from explicit and reproducible sources, so that even chance is repeatable when replayed from the same origin.

Determinism is not a performance optimization or an implementation detail. It is the epistemological foundation of the entire project, and every other principle depends on it. A world is only *believable* if its events can be trusted to have causes; determinism is what makes that trust well-founded. A world is only *studyable* if its histories can be reproduced and examined; determinism is what makes a history more than a single unrepeatable accident. A world can only be *debugged* — its surprises distinguished from its mistakes — if the same conditions reliably yield the same result.

Without determinism, an emergent world is indistinguishable from a broken one. When something strange happens, we could never know whether we had witnessed a genuine consequence of the rules or a failure of the machinery. With determinism, every outcome can be traced, replayed, and understood. The consequence for future development is uncompromising: any feature that introduces irreproducibility into the simulation is not a feature of Genesis. It is a wound in it.

### Article V — Transformation

**Everything is a process. Transformation matters more than state.**

The world is not a collection of things that occasionally change. It is a collection of changes that momentarily present as things. A mountain is erosion caught mid-sentence. A settlement is the ongoing outcome of people continuing to arrive, remain, and depart. A grudge is a process of remembering. What appears to be a stable object is only a process slow enough, at this moment, to look like one.

We elevate transformation above state because state is a photograph and the world is the film. To describe Genesis by listing what exists at an instant is to mistake a single frame for the motion it was cut from. What matters is not the configuration of the world at any given moment but the rules by which one moment becomes the next. Those rules are the world's actual content. The state is merely where they have arrived so far.

The consequence is that Genesis is built to describe change first and configuration second. We ask of every element not "what is it?" but "how does it become, persist, and pass?" A world understood as transformation can have a genuine past and an open future. A world understood as static state can only be edited, never lived.

### Article VI — Causality

**Nothing happens without a cause.**

Every event in Genesis is the consequence of prior conditions and explicit rules acting upon them. There are no uncaused occurrences, no events inserted from outside the world's own logic, no effects that appear because a designer decided the moment was right for them. If something happens, it happened *because* — and that because can always, in principle, be followed back to its origins.

This is what separates an emergent world from a scripted one. In a scripted world, causes are optional; the author may simply declare an outcome and supply a reason afterward, or none at all. In Genesis, the cause is not a justification offered after the fact. It is the actual mechanism by which the event occurred. Remove the cause and the event does not happen, because there was nothing else making it happen.

Causality is what makes the world's surprises *meaningful* rather than merely random. When Genesis produces something no one anticipated, the value lies in the fact that it was nonetheless caused — that a chain of comprehensible steps led there, and can be walked back. A surprising outcome with a traceable cause is discovery. A surprising outcome with no cause is noise. The consequence for future development is that every event must be answerable to the question *why did this occur?* with an answer that lives inside the world's own rules, and never with the answer *because we wanted it to.*

### Article VII — Locality

**Global behavior arises from local interaction.**

The world is governed from the bottom. Its elements act on the basis of their own state and their immediate neighborhood — what is near them, what touches them, what they can directly sense or affect. No element consults a global plan. No element is steered by a central authority arranging the whole toward a predetermined shape. Large-scale structure is not imposed from above; it accumulates from below, out of countless small interactions that individually know nothing of the pattern they compose.

We insist on locality because it is the only known way to produce genuine, unplanned structure. A pattern dictated from a central vantage is limited by the imagination of whoever holds that vantage. A pattern that emerges from local rules is limited only by the richness of the rules themselves, and can exceed anything its designers foresaw. Migration routes, trade networks, the spread of ideas and illnesses and reputations — these are the kinds of large-scale order that locality can grow and that central authorship can only ever fake.

The consequence is a standing prohibition against the god-view shortcut: the temptation, whenever a global outcome is desired, to reach in and arrange it directly. Genesis achieves global outcomes by finding the local rules that produce them, or it does not achieve them at all. This is harder. It is the entire point.

### Article VIII — Emergence

**Systems remain simple. Complexity emerges from their interaction.**

The richness of Genesis is meant to live in the *interactions between* systems, not in the systems themselves. Each individual rule should be simple enough to be held whole in one mind and understood completely. Depth is not achieved by making any single system elaborate. It is achieved by composing simple systems whose combination produces behavior far exceeding the sum of its parts.

This is a deliberate wager about where complexity should be allowed to accumulate. Complexity that we build directly into a system is complexity we must forever maintain, comprehend, and debug, and it can never yield more than we put in. Complexity that emerges from the interaction of simple systems costs us only the simple systems, and returns behavior we did not have to author and often could not have imagined. The first kind of complexity is a debt. The second is a dividend.

The consequence is a constant preference, whenever a system threatens to grow intricate, to ask whether the intricacy could instead arise from the meeting of simpler parts. Genesis distrusts any single system that has become complicated on its own, and trusts complexity only when it is emergent — when it lives in the relationships between simple things rather than inside any one of them. When we are tempted to make a system cleverer, we should first ask whether we could instead make it simpler and let cleverness emerge.

### Article IX — Emergent Narrative

**Stories are discovered, not written.**

Genesis does not contain authored stories, and it does not aim to. What it aims to contain are the conditions from which stories arise on their own: characters with wants and memories, circumstances that put those wants in tension, and consequences that follow honestly from how the tensions resolve. A story, in Genesis, is not a thing placed into the world. It is a pattern found within the world's own history after the fact — a meaningful thread that the simulation produced without ever intending to.

We refuse authored narrative because an authored story is a promise the world cannot keep. The instant a plot is scripted to unfold a certain way, the world must be bent to serve it, and every principle above is compromised in the bending. The character must feel what the plot requires rather than what their circumstances would produce. The event must occur on schedule rather than when its causes align. The story is preserved and the world is falsified.

Genesis accepts the trade that this implies. Emergent stories are less controllable than authored ones. They cannot be guaranteed to arrive on cue, to resolve satisfyingly, or to arrive at all. We accept this because a story that emerges is *true* in a way an authored one can never be — it genuinely happened, for genuine reasons, to characters who were not pretending. The consequence is that Genesis invests in the machinery of meaning — motivation, memory, consequence, relation — and never in the machinery of plot. We build the soil. We do not plant the flowers. What grows, grows.

### Article X — Persistence

**The world remembers.**

What happens in Genesis leaves marks, and those marks endure and compound. Consequences do not evaporate when attention moves elsewhere or time moves on. A wound heals along a particular scar; a decision forecloses the futures that depended on the road not taken; a history accumulates and presses on the present. The world's past is not discarded to make room for its present. Its past is the reason its present has the shape it does.

Persistence is what gives the world weight. A world without memory resets to a kind of eternal, meaningless now, in which nothing that occurs can matter because nothing that occurs can last. Belief depends on the sense that actions have durable consequences — that the world is keeping an honest account, and that the account is what the future will be built upon. Without persistence, the other principles produce events that are true but weightless, real for an instant and then gone.

The consequence for development is that Genesis must treat the accumulation of consequence as a first-class concern, not an afterthought bolted on when a feature happens to need to be remembered. The world's capacity to carry its own past forward is part of what the world *is*. What cannot be remembered cannot truly have mattered.

### Article XI — Observation

**Presentation reveals the world. It never governs it.**

To observe Genesis is to open a window onto a world that does not depend on the window. Any presentation — a rendering, a chart, a written chronicle, a single number reported to a researcher — is a way of reading the world's state, and never a way of writing it. Presentation is downstream of simulation, always, without exception. Information flows from the world to the view. It does not flow back.

This directional discipline is the practical guarantee of every principle above it. The moment a presentation is allowed to influence the simulation — to hold a fact the world does not hold, to shape an event for the sake of how it will appear, to become a place where the observer's needs quietly re-enter and bend the world toward them — the world has acquired a second master and Article I has fallen. Everything the Constitution protects is protected only so long as presentation stays strictly downstream.

The consequence is that Genesis must be able to run with no presentation at all, unchanged in its behavior, and be observed through any number of presentations without any of them altering what it does. A view may be rich or spare, real-time or retrospective, built for a person or a machine; it may come and go, and many may watch at once. Through all of it the world behaves identically, because the world does not know it is being watched. If turning off a view changes what the world does, the view was never a view. It was a hidden hand.

### Article XII — Explicitness

**Data is explicit. Hidden state is forbidden.**

Everything the world's behavior depends upon is part of the world's declared state, expressed openly, available to be inspected. There is no shadow state — no fact that secretly influences outcomes without being part of the account, no memory tucked away where it cannot be examined, no cause that acts from concealment. If it affects the world, it is *in* the world, in the open, where it can be seen, saved, restored, and reasoned about.

Explicitness is the enabling condition of nearly everything else the Constitution requires. Determinism can only be guaranteed if every input to the world's evolution is accounted for; a single hidden dependency is a single place where reproducibility silently breaks. Causality can only be traced if every cause is visible; a concealed cause is an event that appears to come from nowhere. Persistence can only be complete if the whole of the world's relevant state is capturable; what is hidden cannot be reliably remembered. Hidden state is the common failure beneath many separate betrayals.

The consequence is a strict standard applied everywhere: if something influences the simulation, it must be part of the simulation's explicit, inspectable state, with no exceptions made for convenience. The health of the entire system can be measured by a single question — *is there anything affecting this world that the world does not openly declare?* The correct number of such things is zero.

### Article XIII — Composition

**Structure is built by composition, not by inheritance.**

The elements of Genesis are assembled from parts that can be combined freely, rather than derived from ancestors that fix their nature in advance. What a thing is, is determined by what it is composed of and how those components interact — not by its position in a hierarchy of categories decided ahead of time. New kinds of things arise by combining existing components in new arrangements, not by extending a lineage.

We prefer composition because rigid hierarchies of kind are a poor fit for a world meant to emerge. A taxonomy fixed in advance can only express the categories its designers foresaw, and every unanticipated combination fights against it. Composition imposes no such ceiling. When the parts combine freely, the space of possible things is open rather than enumerated, and the world can produce arrangements no one placed in a catalog beforehand. Emergence at the level of structure requires the same freedom that emergence at the level of behavior does.

The consequence is a consistent bias, throughout Genesis, toward describing things as compositions of capabilities rather than as instances of predefined types. When a new kind of thing is needed, the first question is which existing components combine to make it, and only failing that whether a genuinely new component is warranted. This keeps the world's building blocks few, general, and recombinable — and keeps the world itself open.

### Article XIV — Comprehensibility

**The system must remain understandable to those who come later.**

Genesis is a long project, and it will pass through hands that are not ours. Its principles are only as durable as they are understandable. A system that cannot be comprehended cannot be faithfully maintained, and a Constitution that governs an incomprehensible system governs it in name only. We therefore treat the clarity of Genesis — the legibility of its rules, the transparency of its structure, the honesty of its expression — as a principle equal in standing to the rest.

We hold clarity as a first-order commitment because obscurity is where all the other principles quietly go to die. Determinism erodes when no one can any longer see every input. Explicitness erodes when the state is technically visible but practically unintelligible. Simplicity erodes one reasonable-seeming complication at a time, each defensible alone, until no one holds the whole. The defense against all of this is the same: the system must be kept comprehensible, deliberately and continuously, against the natural drift toward complexity.

The consequence is that clarity is not a courtesy extended when time allows. It is a requirement of the work. A rule that cannot be explained is a rule we do not fully understand, and a rule we do not fully understand is one we cannot trust to uphold the principles above. When clarity and cleverness conflict, Genesis chooses clarity, because clarity is what lets Genesis still be Genesis in the hands of people we will never meet.

---

## What Genesis Refuses

The principles above imply their opposites. It is worth naming the opposites directly, because they are the defaults of the field, and defaults reassert themselves unless they are explicitly rejected. Genesis refuses the following, not because they are without value elsewhere, but because each contradicts something the Constitution holds.

**Worlds that revolve around the observer.** Any arrangement in which the world reorganizes itself around whoever is watching — events summoned by their approach, difficulty tuned to their skill, significance assigned by their attention — is refused. It contradicts Articles II and III. A world that is about its observer is not a world; it is a mirror.

**Authored and scripted behavior.** Any mechanism that dictates an outcome rather than causing it — a character made to act against what their circumstances would produce, an event fired on a schedule rather than when its causes align — is refused. It contradicts Articles VI and IX. Scripting is the reintroduction of the second master, one exception at a time.

**Architecture driven by content.** Any structure in which adding new content requires reshaping the fundamental rules — where the world's laws bend to accommodate each new thing placed in it — is refused. It contradicts Articles VIII and XIII. The rules are the constant; the content is what the rules produce, and content must never become the tail that wags the world.

**Unnecessary abstraction.** Any layer of indirection that exists for elegance, symmetry, or the anticipation of needs that have not arrived is refused. It contradicts Articles VIII and XIV. Every abstraction must earn its place by making the world simpler or clearer in fact, not in theory. Abstraction that serves only itself is complexity wearing the costume of order.

**Global mutable state without cause.** Any shared, hidden, freely writable state that lets distant parts of the system influence one another invisibly is refused. It contradicts Articles VII and XII. It is the mechanism by which locality is quietly violated and hidden state quietly enters.

**Systems with concealed behavior.** Any system whose outcomes depend on facts it does not openly declare — magic in the pejorative sense, effects without visible causes — is refused. It contradicts Articles VI and XII. If its workings cannot be inspected, its workings cannot be trusted.

**Features that exist only by convention.** Any feature included because it is expected, customary, or common elsewhere, rather than because it serves the principles of Genesis, is refused. It contradicts the spirit of the whole. Genesis is not obligated to resemble anything. It is obligated only to be coherent with itself.

To refuse these is not austerity for its own sake. Each refusal is the shadow of a commitment, and keeping the commitments requires keeping the refusals.

---

## Decision Framework

When a new system, feature, or change is proposed, it is to be weighed against the Constitution before it is weighed against anything else. Cleverness, convenience, familiarity, and even usefulness are secondary considerations. The primary question is always whether the thing belongs in Genesis at all. The following questions make that judgment concrete. They are not a scoring rubric to be averaged; a decisive failure on any one of them is grounds to reconsider, however well the proposal answers the rest.

- **Does it make the world more believable?** Believability — the sense that the world has genuine causes and consequences of its own — is the end toward which everything else is a means. A proposal that makes the world less believable is working against the project even if it is impressive on its own terms.

- **Does it increase emergence, or does it script?** Does the proposal add to the machinery from which unplanned behavior arises, or does it reach in and dictate an outcome directly? Genesis grows what it wants; it does not place it. A feature that manufactures a result rather than cultivating its causes is suspect no matter how good the result looks.

- **Does it preserve determinism?** Does the same beginning still produce the same world once this exists? A proposal that introduces irreproducibility into the simulation fails at the foundation, and no other merit can compensate, because determinism is the ground the other merits stand on.

- **Does it keep causes and state explicit?** Every new influence on the world must be traceable to a cause and expressed in open, inspectable state. A proposal that introduces a hidden dependency, a concealed effect, or a fact the world does not openly declare is refused until the concealment is removed.

- **Does it reduce complexity, or move it somewhere honest?** Does the proposal make Genesis simpler and clearer, or does it add intricacy? Where complexity is genuinely necessary, does it live in the interaction of simple parts rather than inside a single elaborate one? Complexity that merely accumulates, defensible step by defensible step, is how a comprehensible system stops being one.

- **Would the world still function without the observer?** Turn off every view and remove every participant. Does the proposal still make sense? Anything that only has meaning when someone is watching in the intended way belongs to presentation, not to the world — and if it has been placed in the world, it has been placed wrongly.

A proposal that answers these well belongs in Genesis, whatever its origin. A proposal that answers them poorly does not, however common, convenient, or clever it may be. When the answers are genuinely unclear, that uncertainty is itself information: it usually means the proposal has not yet been understood well enough to build, and the correct response is to understand it further, not to build it sooner.

---

## Final Commitment

Genesis is a long undertaking, and this document is its promise to everyone who will ever work on it — including versions of ourselves we cannot yet imagine, and contributors we will never meet.

The promise is this. We will build a world that is true before it is impressive. We will prefer a world that is real and modest over a world that is spectacular and hollow. We will resist, again and again, the standing temptation to reach into the world and arrange it directly, and we will pay the higher price of finding the rules that let it arrange itself. We will keep the world's causes honest, its state explicit, and its rules simple enough to understand. We will let the world be the authority on itself, and we will keep every window onto it strictly downstream of the thing it observes. We will accept the stories we are given rather than demanding the stories we want, and we will trust that a world built well enough will, in time, surprise even us.

We hold this document above the code that will implement it. Implementations are temporary. If Genesis is one day rewritten in another language, on other machines, by other people, under approaches not yet invented, this Constitution should survive the rewrite unchanged and govern the result exactly as it governs the present. The technology is the current answer to the question of how. The Constitution is the permanent answer to the question of what, and why. Should a single file outlast a complete reconstruction of everything beneath it, let it be this one.

We commit to reconsidering the feature, never the principle — to treating a conflict with this document as a signal that the work has drifted, not that the foundation has aged. And we commit to keeping Genesis comprehensible enough that those who come after can hold these principles as we have tried to, and carry them further than we could.

We are not building a world to be looked at. We are building a world that would go on without us, and choosing, for a while, to watch. 🌱
