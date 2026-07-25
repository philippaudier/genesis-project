using System;
using System.Collections.Generic;

namespace Genesis.Simulation
{
    /// <summary>
    /// A commutative resolver that commits the sum of the conflicting amounts — so contributions of
    /// +2 and +3 resolve to +5, in either order. The witness commutative resolver for Genesis-006.
    /// (DN-001 treats "accumulate" not as a global policy but as one example of a per-kind commutative
    /// resolver; this is that example.)
    /// </summary>
    public sealed class AdditionResolver : IConflictResolver
    {
        public long Resolve(IReadOnlyList<long> amounts)
        {
            if (amounts == null)
            {
                throw new ArgumentNullException(nameof(amounts));
            }

            long sum = 0;
            for (int i = 0; i < amounts.Count; i++)
            {
                sum += amounts[i];
            }

            return sum;
        }
    }
}
