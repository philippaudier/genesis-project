using System.Collections.Generic;

namespace Genesis.Simulation.Lootbound
{
    /// <summary>
    /// The five interpreting laws of the first living world. Each reads external cells the membrane
    /// filled and converts intents into effects — by contributions only (ADR-0001). Every law
    /// consumes the intent it read, valid or not: nothing lingers at the boundary. None of them can
    /// know who produced an intent (provenance-blindness). The trace records `Act`; only these laws
    /// decide whether `Act` meant acquire, repair, swap, stow or nothing (the event/command guard).
    /// </summary>
    internal static class LootboundCells
    {
        internal static Cell P(Place place) => new Cell(place, LootboundWorld.PlayerAt);
        internal static Cell Loc(Place sword) => new Cell(sword, LootboundWorld.Location);
        internal static Cell WearOf(Place sword) => new Cell(sword, LootboundWorld.Wear);
        internal static Cell RepairsOf(Place sword) => new Cell(sword, LootboundWorld.Repairs);
        internal static Cell WoodCell => new Cell(LootboundWorld.Pack, LootboundWorld.Wood);

        /// <summary>The sword currently in hand (Location == 0), or null.</summary>
        internal static Place? CarriedSword(IRelationalStateView view)
        {
            if (view.Read(Loc(LootboundWorld.OldSword)) == 0)
            {
                return LootboundWorld.OldSword;
            }

            if (view.Read(Loc(LootboundWorld.NewSword)) == 0)
            {
                return LootboundWorld.NewSword;
            }

            return null;
        }
    }

    /// <summary>
    /// Walking: a Go intent at a place moves the player marker there — but only across a declared
    /// relation. Space is the graph; there is no other way to move.
    /// </summary>
    public sealed class MoveLaw : ITransition
    {
        private readonly Place _target;
        private readonly Cell _goCell;
        private readonly Cell _hereCell;
        private readonly ReadScope _readScope;
        private readonly RelationScope _relationScope;

        public MoveLaw(Place target)
        {
            _target = target;
            _goCell = new Cell(target, LootboundWorld.Go);
            _hereCell = LootboundCells.P(target);
            _readScope = new ReadScope(_goCell, _hereCell);
            _relationScope = new RelationScope(new[] { target }, new[] { LootboundWorld.PlayerAt });
        }

        public ReadScope ReadScope => _readScope;
        public RelationScope RelationScope => _relationScope;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            long go = view.Read(_goCell);
            if (go <= 0)
            {
                return new Contribution[0];
            }

            var contributions = new List<Contribution> { new Contribution(_goCell, -go) };
            if (view.Read(_hereCell) == 0)
            {
                IReadOnlyList<Relation> outgoing = view.OutgoingRelations(_target);
                for (int i = 0; i < outgoing.Count; i++)
                {
                    Cell neighbour = LootboundCells.P(outgoing[i].Target);
                    if (view.Read(neighbour) == 1)
                    {
                        contributions.Add(new Contribution(neighbour, -1));
                        contributions.Add(new Contribution(_hereCell, 1));
                        break;
                    }
                }
            }

            return contributions;
        }
    }

    /// <summary>
    /// Striking the tree: yields one wood into the pack and wears the carried sword by one.
    /// The world's only source of resources, and the only thing that ages a sword.
    /// </summary>
    public sealed class HarvestLaw : ITransition
    {
        private static readonly Cell AttackCell = new Cell(LootboundWorld.Tree, LootboundWorld.Attack);
        private static readonly Cell AtTree = LootboundCells.P(LootboundWorld.Tree);

        private readonly ReadScope _readScope = new ReadScope(
            AttackCell, AtTree,
            LootboundCells.Loc(LootboundWorld.OldSword), LootboundCells.Loc(LootboundWorld.NewSword));

        public ReadScope ReadScope => _readScope;
        public RelationScope RelationScope => RelationScope.Empty;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            long strikes = view.Read(AttackCell);
            if (strikes <= 0)
            {
                return new Contribution[0];
            }

            var contributions = new List<Contribution> { new Contribution(AttackCell, -strikes) };
            if (view.Read(AtTree) == 1)
            {
                contributions.Add(new Contribution(LootboundCells.WoodCell, 1));
                Place? carried = LootboundCells.CarriedSword(view);
                if (carried.HasValue)
                {
                    contributions.Add(new Contribution(LootboundCells.WearOf(carried.Value), 1));
                }
            }

            return contributions;
        }
    }

    /// <summary>
    /// The repair station: one resource, one gesture. A full repair — wear returns to zero, the
    /// repair count grows by one, one wood is spent. Inert unless the player stands here, carries
    /// a worn sword, and owns wood.
    /// </summary>
    public sealed class RepairLaw : ITransition
    {
        private static readonly Cell ActCell = new Cell(LootboundWorld.Station, LootboundWorld.Act);
        private static readonly Cell AtStation = LootboundCells.P(LootboundWorld.Station);

        private readonly ReadScope _readScope = new ReadScope(
            ActCell, AtStation, LootboundCells.WoodCell,
            LootboundCells.Loc(LootboundWorld.OldSword), LootboundCells.Loc(LootboundWorld.NewSword),
            LootboundCells.WearOf(LootboundWorld.OldSword), LootboundCells.WearOf(LootboundWorld.NewSword));

        public ReadScope ReadScope => _readScope;
        public RelationScope RelationScope => RelationScope.Empty;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            long act = view.Read(ActCell);
            if (act <= 0)
            {
                return new Contribution[0];
            }

            var contributions = new List<Contribution> { new Contribution(ActCell, -act) };
            if (view.Read(AtStation) == 1 && view.Read(LootboundCells.WoodCell) > 0)
            {
                Place? carried = LootboundCells.CarriedSword(view);
                if (carried.HasValue)
                {
                    long wear = view.Read(LootboundCells.WearOf(carried.Value));
                    if (wear > 0)
                    {
                        contributions.Add(new Contribution(LootboundCells.WearOf(carried.Value), -wear));
                        contributions.Add(new Contribution(LootboundCells.RepairsOf(carried.Value), 1));
                        contributions.Add(new Contribution(LootboundCells.WoodCell, -1));
                    }
                }
            }

            return contributions;
        }
    }

    /// <summary>
    /// The clearing: interacting swaps hands and ground — the carried sword is laid down, the one
    /// lying here is taken up. Standing here and walking away instead is the world's way of letting
    /// a refusal exist: no law records it; the laboratory derives it.
    /// </summary>
    public sealed class SwapLaw : ITransition
    {
        private static readonly Cell ActCell = new Cell(LootboundWorld.Clearing, LootboundWorld.Act);
        private static readonly Cell AtClearing = LootboundCells.P(LootboundWorld.Clearing);

        private readonly ReadScope _readScope = new ReadScope(
            ActCell, AtClearing,
            LootboundCells.Loc(LootboundWorld.OldSword), LootboundCells.Loc(LootboundWorld.NewSword));

        public ReadScope ReadScope => _readScope;
        public RelationScope RelationScope => RelationScope.Empty;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            long act = view.Read(ActCell);
            if (act <= 0)
            {
                return new Contribution[0];
            }

            var contributions = new List<Contribution> { new Contribution(ActCell, -act) };
            if (view.Read(AtClearing) == 1)
            {
                Place? onGround = null;
                foreach (Place sword in LootboundWorld.Swords)
                {
                    if (view.Read(LootboundCells.Loc(sword)) == LootboundWorld.Clearing.Value)
                    {
                        onGround = sword;
                        break;
                    }
                }

                if (onGround.HasValue)
                {
                    contributions.Add(new Contribution(LootboundCells.Loc(onGround.Value), -LootboundWorld.Clearing.Value));
                    Place? carried = LootboundCells.CarriedSword(view);
                    if (carried.HasValue)
                    {
                        contributions.Add(new Contribution(LootboundCells.Loc(carried.Value), LootboundWorld.Clearing.Value));
                    }
                }
            }

            return contributions;
        }
    }

    /// <summary>
    /// The shelter's chest: interacting stows the carried sword; empty-handed, it takes the stowed
    /// one back (lowest place id first — canonical, deterministic). The chest exists to answer one
    /// question: is this sword kept?
    /// </summary>
    public sealed class StowLaw : ITransition
    {
        private static readonly Cell ActCell = new Cell(LootboundWorld.Shelter, LootboundWorld.Act);
        private static readonly Cell AtShelter = LootboundCells.P(LootboundWorld.Shelter);

        private readonly ReadScope _readScope = new ReadScope(
            ActCell, AtShelter,
            LootboundCells.Loc(LootboundWorld.OldSword), LootboundCells.Loc(LootboundWorld.NewSword));

        public ReadScope ReadScope => _readScope;
        public RelationScope RelationScope => RelationScope.Empty;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            long act = view.Read(ActCell);
            if (act <= 0)
            {
                return new Contribution[0];
            }

            var contributions = new List<Contribution> { new Contribution(ActCell, -act) };
            if (view.Read(AtShelter) == 1)
            {
                Place? carried = LootboundCells.CarriedSword(view);
                if (carried.HasValue)
                {
                    contributions.Add(new Contribution(LootboundCells.Loc(carried.Value), LootboundWorld.Shelter.Value));
                }
                else
                {
                    foreach (Place sword in LootboundWorld.Swords) // ascending id: canonical order
                    {
                        if (view.Read(LootboundCells.Loc(sword)) == LootboundWorld.Shelter.Value)
                        {
                            contributions.Add(new Contribution(LootboundCells.Loc(sword), -LootboundWorld.Shelter.Value));
                            break;
                        }
                    }
                }
            }

            return contributions;
        }
    }
}
