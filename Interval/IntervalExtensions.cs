using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intervals
{
    public static class IntervalExtensions
    {
        public static int Start(this IInterval interval)
        {
            return interval.ToInterval().Start;
        }

        public static int End(this IInterval interval)
        {
            return interval.ToInterval().End;
        }

        public static int Length(this IInterval interval)
        {
            return interval.ToInterval().Length;
        }

        public static IEnumerable<Interval> ToIntervals(this IEnumerable<IInterval> intervals)
        {
            return intervals.Select(i => i.ToInterval());
        }

        public static bool IsEmpty(this Interval self)
        {
            return self.Length == 0;
        }


        public static bool IsBefore(this IInterval self, IInterval other,
            ContainsMode mode = ContainsMode.NON_STRICT)
        {
            var s = self.ToInterval();
            var o = other.ToInterval();

            if (mode == ContainsMode.NON_STRICT)
                return s.End <= o.Start;
            else
                return s.End < o.Start;
        }

        public static bool IsAfter(this IInterval self, IInterval other,
            ContainsMode mode = ContainsMode.NON_STRICT)
        {
            var s = self.ToInterval();
            var o = other.ToInterval();

            if (mode == ContainsMode.NON_STRICT)
                return s.Start >= o.End;
            else
                return s.Start > o.End;
        }

        public static bool Meets(this IInterval self, IInterval other)
        {
            var s = self.ToInterval();
            var o = other.ToInterval();

            return s.End == o.Start;
        }

        //public static bool Overlaps(this ISegment self, ISegment other)
        //{
        //    return self.Start < other.Start && other.Start < self.End;
        //}

        public static bool Starts(this IInterval self, IInterval other)
        {
            var s = self.ToInterval();
            var o = other.ToInterval();

            return s.Start == o.Start && s.End < o.End;
        }

        public static bool Finishes(this IInterval self, IInterval other)
        {
            var s = self.ToInterval();
            var o = other.ToInterval();

            return s.Start > o.Start && s.End == o.End;
        }

        public static bool IsMergedWith(this IInterval self, IInterval other)
        {
            var s = self.ToInterval();
            var o = other.ToInterval();

            return s.Start == o.Start && o.End == s.End;
        }

        public static bool Contains(this IInterval self, IInterval other,
            ContainsMode mode = ContainsMode.NON_STRICT)
        {
            var s = self.ToInterval();
            var o = other.ToInterval();

            if (mode == ContainsMode.NON_STRICT)
                return s.Start <= o.Start && o.End <= s.End;
            else
                return s.Start < o.Start && o.End < s.End;
        }

        public static bool Contains(this IInterval self, int index,
            ContainsMode mode = ContainsMode.NON_STRICT)
        {
            var s = self.ToInterval();

            if (mode == ContainsMode.NON_STRICT)
                return s.Start <= index && index <= s.End;
            else
                return s.Start < index && index < s.End;
        }



        public static Interval Range(this IList<IInterval> intervals)
        {
            var ix = intervals.ToIntervals().ToList();
            var start = ix.Min(i => i.Start);
            var end = ix.Max(i => i.End);
            return new Interval(start, end - start);
        }

        public static int[] Boundaries(this IEnumerable<IInterval> intervals)
        {
            var boundaries = new HashSet<int>();
            foreach (var interval in intervals.ToIntervals())
            {
                boundaries.Add(interval.Start);
                boundaries.Add(interval.End);
            }
            return boundaries.OrderBy(b => b).ToArray();
        }


        public static T[] Slice<T>(this IInterval interval, IEnumerable<T> list)
        {
            var i = interval.ToInterval();
            return list.Skip(i.Start).Take(i.Length).ToArray();
        }

        public static string Slice(this IInterval interval, string s)
        {
            var i = interval.ToInterval();
            return s.Substring(i.Start, i.Length);
        }
    }
}
