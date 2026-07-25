using System.Reflection;
using NUnit.Framework;
using Genesis.Simulation;

namespace Genesis.Tests
{
    /// <summary>
    /// The core Snapshot Transition Model proof (Genesis-004, kept as a regression guard): a
    /// deterministic transition transforms state over any number of ticks, producing the next state
    /// each tick, never exposing a within-tick mutation.
    /// </summary>
    public sealed class SnapshotTransitionModelTests
    {
        private static readonly ITransition[] IncrementA =
        {
            new AddToCounterTransition(TestAddresses.A, 1)
        };

        private static TickRunner NewRunner()
        {
            return new TickRunner(new TransitionRunner());
        }

        [Test]
        public void Two_Runs_Over_1000_Ticks_End_Strictly_Identical()
        {
            TickRunner runner = NewRunner();

            SimulationState first = runner.Run(TestAddresses.InitialState(), IncrementA, 1000);
            SimulationState second = runner.Run(TestAddresses.InitialState(), IncrementA, 1000);

            Assert.AreEqual(first, second);
        }

        [Test]
        public void After_N_Ticks_The_Tick_And_Counter_Are_Both_N()
        {
            const long n = 1000;

            SimulationState result = NewRunner().Run(TestAddresses.InitialState(), IncrementA, n);

            Assert.AreEqual(n, result.CurrentTick.Value);
            Assert.AreEqual(n, result.CounterOf(TestAddresses.A));
        }

        [Test]
        public void Running_Never_Mutates_The_Initial_State()
        {
            SimulationState initial = TestAddresses.InitialState();

            NewRunner().Run(initial, IncrementA, 1000);

            Assert.AreEqual(TestAddresses.InitialState(), initial);
        }

        [Test]
        public void Running_Zero_Ticks_Returns_The_Initial_State()
        {
            SimulationState result = NewRunner().Run(TestAddresses.InitialState(), IncrementA, 0);

            Assert.AreEqual(TestAddresses.InitialState(), result);
        }

        [Test]
        public void A_Tick_With_No_Transitions_Still_Advances_Time()
        {
            SimulationState result = NewRunner().Run(TestAddresses.InitialState(), new ITransition[0], 10);

            Assert.AreEqual(10L, result.CurrentTick.Value);
            Assert.AreEqual(0L, result.CounterOf(TestAddresses.A));
        }

        [Test]
        public void Progression_Is_Deterministic_Tick_For_Tick()
        {
            long[] first = RecordCounterPerTick(500);
            long[] second = RecordCounterPerTick(500);

            CollectionAssert.AreEqual(first, second);
        }

        [Test]
        public void Simulation_Assembly_Has_No_UnityEngine_Dependency()
        {
            Assembly simulation = typeof(SimulationState).Assembly;

            foreach (AssemblyName referenced in simulation.GetReferencedAssemblies())
            {
                StringAssert.DoesNotStartWith("UnityEngine", referenced.Name);
                StringAssert.DoesNotStartWith("UnityEditor", referenced.Name);
            }
        }

        private static long[] RecordCounterPerTick(int n)
        {
            TickRunner runner = NewRunner();
            SimulationState state = TestAddresses.InitialState();
            var counters = new long[n];

            for (int i = 0; i < n; i++)
            {
                state = runner.Run(state, IncrementA, 1);
                counters[i] = state.CounterOf(TestAddresses.A);
            }

            return counters;
        }
    }
}
