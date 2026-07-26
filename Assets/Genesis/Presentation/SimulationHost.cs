using Genesis.Simulation;

namespace Genesis.Presentation
{
    /// <summary>
    /// Owns the running world on the presentation side. It decides <em>when</em> ticks execute —
    /// never what a tick does — and publishes each resulting immutable snapshot. Observers read
    /// <see cref="Current"/>; because <see cref="SimulationState"/> is immutable, no observer can
    /// write through it (Genesis-013, Invariant 1).
    /// </summary>
    public sealed class SimulationHost
    {
        private DemoWorldDefinition _world;
        private TickRunner _runner;
        private float _accumulator;

        /// <summary>Wall-clock seconds per tick — a playback rate, never a property of the world.</summary>
        public float SecondsPerTick = 0.5f;

        public bool Playing;

        public SimulationHost()
        {
            Reset();
        }

        /// <summary>The latest published snapshot. Immutable; safe to hand to any observer.</summary>
        public SimulationState Current { get; private set; }

        public DemoWorldDefinition World => _world;

        /// <summary>Advances exactly one tick.</summary>
        public void Step()
        {
            Current = _runner.Run(Current, _world.Relations, _world.Laws, 1);
        }

        /// <summary>Consumes wall-clock time while playing; executes whole ticks only.</summary>
        public void Advance(float deltaSeconds)
        {
            if (!Playing)
            {
                return;
            }

            _accumulator += deltaSeconds;
            while (_accumulator >= SecondsPerTick)
            {
                _accumulator -= SecondsPerTick;
                Step();
            }
        }

        /// <summary>Rebuilds the initial world. Determinism makes this a true reset, not an approximation.</summary>
        public void Reset()
        {
            _world = DemoWorld.Build();
            _runner = new TickRunner(new TransitionRunner(_world.Resolvers));
            Current = _world.InitialState;
            _accumulator = 0f;
            Playing = false;
        }
    }
}
