using System;
using System.Collections.Generic;
using Genesis.Simulation;

namespace Genesis.Lab.S1_001
{
    /// <summary>
    /// A fixture: an experimental law bundle — transitions plus the resolver bindings its kinds
    /// need. Fixtures are instruments, not corpus: nothing enters Genesis by appearing here.
    /// </summary>
    public interface IFixture
    {
        IReadOnlyList<ITransition> Transitions { get; }
        IReadOnlyDictionary<Kind, IConflictResolver> Resolvers { get; }
    }

    /// <summary>
    /// Fixture Transparency (S1-001 post-seal filing): the harness executes a FixtureSet and never
    /// knows which fixtures compose it. Replacing a law is a substitution here, never a rewrite of
    /// the laboratory. Two fixtures binding the same kind to different resolvers is a construction
    /// error — resolver identity per kind is D2's uniformity, kept honest at the door.
    /// </summary>
    public sealed class FixtureSet : IFixture
    {
        private readonly List<ITransition> _transitions = new List<ITransition>();
        private readonly Dictionary<Kind, IConflictResolver> _resolvers = new Dictionary<Kind, IConflictResolver>();

        public FixtureSet(params IFixture[] fixtures)
        {
            foreach (IFixture fixture in fixtures)
            {
                _transitions.AddRange(fixture.Transitions);
                foreach (KeyValuePair<Kind, IConflictResolver> binding in fixture.Resolvers)
                {
                    if (_resolvers.TryGetValue(binding.Key, out IConflictResolver existing) && !ReferenceEquals(existing, binding.Value))
                    {
                        throw new InvalidOperationException($"Kind {binding.Key} bound to two different resolvers.");
                    }

                    _resolvers[binding.Key] = binding.Value;
                }
            }
        }

        public IReadOnlyList<ITransition> Transitions => _transitions;
        public IReadOnlyDictionary<Kind, IConflictResolver> Resolvers => _resolvers;
    }
}
