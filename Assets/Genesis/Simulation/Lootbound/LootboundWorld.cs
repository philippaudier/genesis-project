using System.Collections.Generic;

namespace Genesis.Simulation.Lootbound
{
    /// <summary>
    /// L-002 — the first living world. World content, not kernel: the smallest world able to
    /// produce a biography. Five spatial places on a star graph (walking = crossing a relation),
    /// two swords addressed as places (a place is a unit of addressing, not necessarily spatial —
    /// RFC-0003), one pack, one resource. All influence enters through the membrane (RFC-L001;
    /// ADR-0005): three external kinds, five interpreting laws, nothing else. The world is
    /// deliberately boring — its mission is not to be fun; it is to answer: can a biography be
    /// observed?
    /// </summary>
    public static class LootboundWorld
    {
        // ------------------------------------------------------------------ places
        public static readonly Place Shelter = new Place(100);
        public static readonly Place Tree = new Place(200);
        public static readonly Place Station = new Place(300);
        public static readonly Place Clearing = new Place(400);
        public static readonly Place Field = new Place(500);
        public static readonly Place OldSword = new Place(1000);
        public static readonly Place NewSword = new Place(2000);
        public static readonly Place Pack = new Place(3000);

        public static readonly Place[] Spatial = { Shelter, Tree, Station, Clearing, Field };
        public static readonly Place[] Swords = { OldSword, NewSword };

        // ------------------------------------------------------------------ kinds (internal state)
        /// <summary>1 at exactly one spatial place: where body A stands. (The world knows bodies,
        /// never who drives them — a human, a bot, a replay: provenance-blindness, RFC-L001.)</summary>
        public static readonly Kind PlayerAt = new Kind(1);
        public static readonly Kind Wear = new Kind(2);
        public static readonly Kind Repairs = new Kind(3);
        /// <summary>On swords: the spatial place id where it lies; 0 = held by body A;
        /// 1 = held by body B (declared convention — 1 is not a place id).</summary>
        public static readonly Kind Location = new Kind(4);
        public static readonly Kind Wood = new Kind(5);
        /// <summary>1 at at most one spatial place: where body B stands; all zero = absent
        /// (L-006: the world carries two bodies; a body may leave).</summary>
        public static readonly Kind BodyB = new Kind(6);

        /// <summary>Location value meaning "held by body A".</summary>
        public const long HeldByA = 0;
        /// <summary>Location value meaning "held by body B".</summary>
        public const long HeldByB = 1;

        // ------------------------------------------------------------------ kinds (the membrane)
        /// <summary>External: 1 at a spatial place = body A's driver intends to walk THERE.</summary>
        public static readonly Kind Go = new Kind(11);
        /// <summary>External: 1 at a spatial place = body A interacts here (meaning belongs to that place's law).</summary>
        public static readonly Kind Act = new Kind(12);
        /// <summary>External: 1 at the tree = body A strikes it.</summary>
        public static readonly Kind Attack = new Kind(13);
        /// <summary>External: 1 at a spatial place = body B's driver intends to walk THERE.</summary>
        public static readonly Kind GoB = new Kind(21);
        /// <summary>External: 1 at a spatial place = body B picks up / puts down here (toggle law).</summary>
        public static readonly Kind ActB = new Kind(22);
        /// <summary>External: 1 at the field = body B leaves the world (departure drops what is held).</summary>
        public static readonly Kind LeaveB = new Kind(23);
        /// <summary>External: 1 at the field = body B re-enters the world, if absent (L-007:
        /// the same body, the same identity — the world was never reset in between).</summary>
        public static readonly Kind ArriveB = new Kind(24);

        public static Membrane BuildMembrane()
        {
            return new Membrane(new[] { Go, Act, Attack, GoB, ActB, LeaveB, ArriveB });
        }

        /// <summary>
        /// Tick zero. The player is home; the old sword rests in the shelter's chest (its first
        /// retrieval IS the acquisition); the better sword lies in the clearing; everything else
        /// is zero.
        /// </summary>
        public static SimulationState BuildInitialState()
        {
            var cells = new Dictionary<Cell, long>();
            foreach (Place place in Spatial)
            {
                cells[new Cell(place, PlayerAt)] = place == Shelter ? 1 : 0;
                cells[new Cell(place, Go)] = 0;
                cells[new Cell(place, BodyB)] = place == Tree ? 1 : 0;
                cells[new Cell(place, GoB)] = 0;
                cells[new Cell(place, ActB)] = 0;
            }

            cells[new Cell(Shelter, Act)] = 0;
            cells[new Cell(Station, Act)] = 0;
            cells[new Cell(Clearing, Act)] = 0;
            cells[new Cell(Tree, Attack)] = 0;
            cells[new Cell(Field, LeaveB)] = 0;
            cells[new Cell(Field, ArriveB)] = 0;

            foreach (Place sword in Swords)
            {
                cells[new Cell(sword, Wear)] = 0;
                cells[new Cell(sword, Repairs)] = 0;
            }

            cells[new Cell(OldSword, Location)] = Shelter.Value;
            cells[new Cell(NewSword, Location)] = Clearing.Value;
            cells[new Cell(Pack, Wood)] = 0;

            return new SimulationState(Tick.Zero, cells);
        }

        /// <summary>The star: every location touches the field; walking is one relation at a time.</summary>
        public static RelationSet BuildRelations(SimulationState state)
        {
            var relations = new List<Relation>();
            foreach (Place place in new[] { Shelter, Tree, Station, Clearing })
            {
                relations.Add(new Relation(Field, place));
                relations.Add(new Relation(place, Field));
            }

            return new RelationSet(state, relations.ToArray());
        }

        /// <summary>The interpreting laws — the honest price of RFC-L001, paid for both bodies.</summary>
        public static IReadOnlyList<ITransition> BuildLaws()
        {
            var laws = new List<ITransition>();
            foreach (Place place in Spatial)
            {
                laws.Add(new MoveLaw(place, PlayerAt, Go));
                laws.Add(new MoveLaw(place, BodyB, GoB));
                laws.Add(new PickDropLaw(place));
            }

            laws.Add(new HarvestLaw());
            laws.Add(new RepairLaw());
            laws.Add(new SwapLaw());
            laws.Add(new StowLaw());
            laws.Add(new DepartLaw());
            laws.Add(new ArriveLaw());
            return laws;
        }

        public static TickRunner BuildRunner()
        {
            var resolvers = new Dictionary<Kind, IConflictResolver>
            {
                { PlayerAt, new AdditionResolver() },
                { Wear, new AdditionResolver() },
                { Repairs, new AdditionResolver() },
                { Location, new AdditionResolver() },
                { Wood, new AdditionResolver() },
                { BodyB, new AdditionResolver() },
                { Go, new AdditionResolver() },
                { Act, new AdditionResolver() },
                { Attack, new AdditionResolver() },
                { GoB, new AdditionResolver() },
                { ActB, new AdditionResolver() },
                { LeaveB, new AdditionResolver() },
                { ArriveB, new AdditionResolver() }
            };
            return new TickRunner(new TransitionRunner(resolvers));
        }
    }
}
