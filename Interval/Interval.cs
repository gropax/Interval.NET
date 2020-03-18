using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intervals
{
    public class Interval : IInterval, IEquatable<IInterval>
    {
        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        public Interval(Interval interval)
        {
            Start = interval.Start;
            Length = interval.Length;
        }

        public Interval(int start, int length)
        {
            if (length < 0)
                throw new ArgumentException("Interval length can't be less than 0.");

            Start = start;
            Length = length;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Interval;
            if (other == null)
                return false;

            return Start == other.Start && Length == other.Length;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 17 * Start.GetHashCode() + 19 * End.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"Interval([{Start}, {End}])";
        }

        public Interval ToInterval()
        {
            return this;
        }

        public bool Equals(IInterval other)
        {
            if (other == null) return false;
            var i = other.ToInterval();
            return Start == i.Start && Length == i.Length;
        }

        public static implicit operator Range(Interval i)
        {
            return new Range(i.Start, i.End);
        }
    }


    public class Interval<T> : Interval, IInterval<T>
    {
        public T Value { get; }

        public Interval(Interval interval, T value) : base (interval)
        {
            Value = value;
        }

        public Interval(int start, int length, T value) : base (start, length)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 17 * Start.GetHashCode() + 19 * End.GetHashCode() + 23 * Value.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"Interval([{Start}, {End}], Content = {Value})";
        }

        public new Interval<T> ToInterval()
        {
            return this;
        }

        Interval IInterval.ToInterval()
        {
            return new Interval(this);
        }
    }
}
