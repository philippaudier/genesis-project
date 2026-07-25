using System;

namespace Genesis.Simulation
{
    /// <summary>
    /// A point in simulation time, measured as an ordinal count of ticks from the start of a run.
    /// A tick is a logical step, not a duration (invariant 5): it carries no relationship to real
    /// time. <see cref="Value"/> is an integer index — this is deliberately distinct from the
    /// world's numeric representation, which remains an open RFC decision and is not settled here.
    /// </summary>
    public readonly struct Tick : IEquatable<Tick>, IComparable<Tick>
    {
        /// <summary>The first tick of any run.</summary>
        public static readonly Tick Zero = new Tick(0);

        /// <summary>The ordinal position of this tick, counted from zero. Never negative.</summary>
        public long Value { get; }

        public Tick(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "A tick index cannot be negative.");
            }

            Value = value;
        }

        /// <summary>Returns the tick immediately after this one. Pure; nothing is mutated.</summary>
        public Tick Next()
        {
            return new Tick(Value + 1);
        }

        public bool Equals(Tick other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Tick other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(Tick other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return $"Tick({Value})";
        }

        public static bool operator ==(Tick left, Tick right) => left.Equals(right);
        public static bool operator !=(Tick left, Tick right) => !left.Equals(right);
        public static bool operator <(Tick left, Tick right) => left.Value < right.Value;
        public static bool operator >(Tick left, Tick right) => left.Value > right.Value;
        public static bool operator <=(Tick left, Tick right) => left.Value <= right.Value;
        public static bool operator >=(Tick left, Tick right) => left.Value >= right.Value;
    }
}
