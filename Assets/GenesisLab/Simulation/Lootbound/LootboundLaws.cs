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
    /// Walking: a Go intent at a place moves a body's marker there — but only across a declared
    /// relation. Space is the graph; there is no other way to move. One law, any body (L-006):
    /// the law is instantiated per (place, body kind, intent kind) and never knows who drives.
    /// </summary>
    public sealed class MoveLaw : ITransition
    {
        private readonly Place _target;
        private readonly Kind _bodyKind;
        private readonly Cell _goCell;
        private readonly Cell _hereCell;
        private readonly ReadScope _readScope;
        private readonly RelationScope _relationScope;

        public MoveLaw(Place target, Kind bodyKind, Kind goKind)
        {
            _target = target;
            _bodyKind = bodyKind;
            _goCell = new Cell(target, goKind);
            _hereCell = new Cell(target, bodyKind);
            _readScope = new ReadScope(_goCell, _hereCell);
            _relationScope = new RelationScope(new[] { target }, new[] { bodyKind });
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
                    Cell neighbour = new Cell(outgoing[i].Target, _bodyKind);
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
    /// Body B's one gesture (L-006): at a place, ActB is a toggle — holding nothing, pick up the
    /// sword lying here (lowest id first, canonical); holding something, put it down here. The
    /// meaning of the gesture lives in this law, never in the trace.
    /// </summary>
    public sealed class PickDropLaw : ITransition
    {
        private readonly Place _place;
        private readonly Cell _actCell;
        private readonly Cell _hereCell;
        private readonly ReadScope _readScope;

        public PickDropLaw(Place place)
        {
            _place = place;
            _actCell = new Cell(place, LootboundWorld.ActB);
            _hereCell = new Cell(place, LootboundWorld.BodyB);
            _readScope = new ReadScope(
                _actCell, _hereCell,
                LootboundCells.Loc(LootboundWorld.OldSword), LootboundCells.Loc(LootboundWorld.NewSword));
        }

        public ReadScope ReadScope => _readScope;
        public RelationScope RelationScope => RelationScope.Empty;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            long act = view.Read(_actCell);
            if (act <= 0)
            {
                return new Contribution[0];
            }

            var contributions = new List<Contribution> { new Contribution(_actCell, -act) };
            if (view.Read(_hereCell) == 1)
            {
                Place? held = null;
                Place? onGround = null;
                foreach (Place sword in LootboundWorld.Swords) // ascending id: canonical order
                {
                    long loc = view.Read(LootboundCells.Loc(sword));
                    if (loc == LootboundWorld.HeldByB && !held.HasValue)
                    {
                        held = sword;
                    }

                    if (loc == _place.Value && !onGround.HasValue)
                    {
                        onGround = sword;
                    }
                }

                if (held.HasValue)
                {
                    contributions.Add(new Contribution(
                        LootboundCells.Loc(held.Value), _place.Value - LootboundWorld.HeldByB));
                }
                else if (onGround.HasValue)
                {
                    contributions.Add(new Contribution(
                        LootboundCells.Loc(onGround.Value), LootboundWorld.HeldByB - _place.Value));
                }
            }

            return contributions;
        }
    }

    /// <summary>
    /// Departure (L-006): body B may leave the world, from the field only. Departure may not
    /// destroy — whatever the body holds is put down where it stands; then the marker goes to
    /// zero. Objects are conserved; only drivers leave.
    /// </summary>
    public sealed class DepartLaw : ITransition
    {
        private static readonly Cell LeaveCell = new Cell(LootboundWorld.Field, LootboundWorld.LeaveB);
        private static readonly Cell AtField = new Cell(LootboundWorld.Field, LootboundWorld.BodyB);

        private readonly ReadScope _readScope = new ReadScope(
            LeaveCell, AtField,
            LootboundCells.Loc(LootboundWorld.OldSword), LootboundCells.Loc(LootboundWorld.NewSword));

        public ReadScope ReadScope => _readScope;
        public RelationScope RelationScope => RelationScope.Empty;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            long leave = view.Read(LeaveCell);
            if (leave <= 0)
            {
                return new Contribution[0];
            }

            var contributions = new List<Contribution> { new Contribution(LeaveCell, -leave) };
            if (view.Read(AtField) == 1)
            {
                foreach (Place sword in LootboundWorld.Swords)
                {
                    if (view.Read(LootboundCells.Loc(sword)) == LootboundWorld.HeldByB)
                    {
                        contributions.Add(new Contribution(
                            LootboundCells.Loc(sword), LootboundWorld.Field.Value - LootboundWorld.HeldByB));
                    }
                }

                contributions.Add(new Contribution(AtField, -1));
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
    /// The station's gesture: one resource, one act. When a repair is possible — a worn sword in
    /// hand, wood in the pack — a full repair: wear returns to zero, the repair count grows by
    /// one, one wood is spent. **Otherwise (L-008), the previously dead case is lifted, not
    /// added to:** the act exchanges with the ground, exactly as the clearing's gesture does —
    /// what is carried is laid down, what lies here is taken up. No new law, no new kind, no new
    /// cell: everything this branch reads was already in this law's declared scope. Every
    /// previously possible outcome is unchanged; only the case that used to do nothing now does
    /// something.
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
            if (view.Read(AtStation) == 1)
            {
                Place? carried = LootboundCells.CarriedSword(view);
                long wear = carried.HasValue ? view.Read(LootboundCells.WearOf(carried.Value)) : 0;
                bool repairPossible = carried.HasValue && wear > 0 && view.Read(LootboundCells.WoodCell) > 0;

                if (repairPossible)
                {
                    contributions.Add(new Contribution(LootboundCells.WearOf(carried.Value), -wear));
                    contributions.Add(new Contribution(LootboundCells.RepairsOf(carried.Value), 1));
                    contributions.Add(new Contribution(LootboundCells.WoodCell, -1));
                }
                else
                {
                    // L-008 — the lifted case: exchange with the ground, the clearing's gesture.
                    Place? onGround = null;
                    foreach (Place sword in LootboundWorld.Swords) // ascending id: canonical order
                    {
                        if (view.Read(LootboundCells.Loc(sword)) == LootboundWorld.Station.Value)
                        {
                            onGround = sword;
                            break;
                        }
                    }

                    if (onGround.HasValue)
                    {
                        contributions.Add(new Contribution(
                            LootboundCells.Loc(onGround.Value), LootboundWorld.HeldByA - LootboundWorld.Station.Value));
                        if (carried.HasValue)
                        {
                            contributions.Add(new Contribution(
                                LootboundCells.Loc(carried.Value), LootboundWorld.Station.Value - LootboundWorld.HeldByA));
                        }
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

    /// <summary>
    /// Arrival (L-007): body B may re-enter the world, at the field only, and only while absent.
    /// The same marker, the same identity — nothing was reset while it was away; the world simply
    /// kept being the world. Arrival creates nothing and carries nothing in: the body returns
    /// empty-handed to whatever it left.
    /// </summary>
    public sealed class ArriveLaw : ITransition
    {
        private static readonly Cell ArriveCell = new Cell(LootboundWorld.Field, LootboundWorld.ArriveB);

        private readonly ReadScope _readScope;

        public ArriveLaw()
        {
            var cells = new List<Cell> { ArriveCell };
            foreach (Place place in LootboundWorld.Spatial)
            {
                cells.Add(new Cell(place, LootboundWorld.BodyB));
            }

            _readScope = new ReadScope(cells.ToArray());
        }

        public ReadScope ReadScope => _readScope;
        public RelationScope RelationScope => RelationScope.Empty;

        public IReadOnlyList<Contribution> Apply(IRelationalStateView view)
        {
            long arrive = view.Read(ArriveCell);
            if (arrive <= 0)
            {
                return new Contribution[0];
            }

            var contributions = new List<Contribution> { new Contribution(ArriveCell, -arrive) };
            bool anywhere = false;
            foreach (Place place in LootboundWorld.Spatial)
            {
                if (view.Read(new Cell(place, LootboundWorld.BodyB)) == 1)
                {
                    anywhere = true;
                }
            }

            if (!anywhere)
            {
                contributions.Add(new Contribution(new Cell(LootboundWorld.Field, LootboundWorld.BodyB), 1));
            }

            return contributions;
        }
    }
}
