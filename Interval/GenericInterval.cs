using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intervals
{


    public interface IIntervalInclusionComparer
    {
        bool Contains(IInterval i1, IInterval i2);
    }

    public static class InclusionComparer
    {
        public static LooseInclusionComparer Loose { get => new LooseInclusionComparer(); }
        public static StrictInclusionComparer Strict { get => new StrictInclusionComparer(); }
    }

    public class LooseInclusionComparer : IIntervalInclusionComparer
    {
        public bool Contains(IInterval interval1, IInterval interval2)
        {
            var i1 = interval1.ToInterval();
            var i2 = interval2.ToInterval();
            return i1.Start <= i2.Start && i2.End <= i1.End;
        }
    }

    public class StrictInclusionComparer : IIntervalInclusionComparer
    {
        public bool Contains(IInterval interval1, IInterval interval2)
        {
            var i1 = interval1.ToInterval();
            var i2 = interval2.ToInterval();
            return i1.Length > i2.Length && (i1.Start <= i2.Start && i2.End <= i1.End);
        }
    }


    public enum ContainsMode
    {
        STRICT,
        NON_STRICT,
    }

    public static class GenericIntervalExtensions
    {
        public class Annotation : IInterval<Annotation>
        {
            public int Start { get; private set; }
            public int Length { get; private set; }
            public string Content { get; private set; }

            public Annotation(int start, int length, string value)
            {
                Start = start;
                Length = length;
                Content = value;
            }

            public Interval<Annotation> ToInterval()
            {
                return new Interval<Annotation>(Start, Length, this);
            }

            Interval IInterval.ToInterval()
            {
                return new Interval(Start, Length);
            }
        }

        public static void Main()
        {
            var annotation = new Annotation(10, 5, "youpi");
            annotation.IsMergedWith(annotation);
            var interval = annotation.ToInterval();
            int start = interval.Start;
            int start2 = annotation.Start();
        }


        public static IEnumerable<Interval<T>> ToIntervals<T>(this IEnumerable<IInterval<T>> intervals)
        {
            return intervals.Select(i => i.ToInterval());
        }

        public static T Value<T>(this IInterval<T> interval)
        {
            return interval.ToInterval().Value;
        }

        public static IEnumerable<T> Values<T>(this IEnumerable<Interval<T>> intervals)
        {
            return intervals.Select(i => i.Value);
        }

        public static bool IsEmpty<T>(this IInterval<T> self)
        {
            return self.ToInterval().Length == 0;
        }

        public static Interval<T> Extend<T>(this IInterval<T> interval, int length)
        {
            var i = interval.ToInterval();
            return new Interval<T>(i.Start - length, i.Length + 2 * length, i.Value);
        }

        public static Interval<T> ExtendLeft<T>(this IInterval<T> interval, int length)
        {
            var i = interval.ToInterval();
            return new Interval<T>(i.Start - length, i.Length + length, i.Value);
        }

        public static Interval<T> ExtendRight<T>(this IInterval<T> interval, int length)
        {
            var i = interval.ToInterval();
            return new Interval<T>(i.Start, i.Length + length, i.Value);
        }

        public static Interval<T> Translate<T>(this IInterval<T> interval, int value)
        {
            var i = interval.ToInterval();
            return new Interval<T>(i.Start + value, i.Length, i.Value);
        }

        public static IEnumerable<Interval<T>> Translate<T>(this IEnumerable<IInterval<T>> intervals, int value)
        {
            return intervals.Select(i => i.Translate(value));
        }

        public static Interval<T[]> Range<T>(this IList<IInterval<T>> intervals)
        {
            var ix = intervals.ToIntervals().ToList();
            var start = ix.Min(i => i.Start);
            var end = ix.Max(i => i.End);
            return new Interval<T[]>(start, end - start, ix.Select(i => i.Value).ToArray());
        }

        public static Interval<U> Range<T, U>(this IList<IInterval<T>> intervals, Func<IEnumerable<T>, U> reduce)
        {
            var ix = intervals.ToIntervals().ToList();
            var start = ix.Min(i => i.Start);
            var end = ix.Max(i => i.End);
            return new Interval<U>(start, end - start, reduce(ix.Select(i => i.Value)));
        }

        //public static SortedIntervals<T> Sorted<T>(this IEnumerable<IInterval<T>> intervals)
        //{
        //    return new SortedIntervals<T>(intervals.ToIntervals()
        //        .OrderBy(i => i.Start).ThenByDescending(i => i.Length));
        //}

        public static IEnumerable<Interval<T>> Map<T>(this IEnumerable<IInterval> intervals, Func<IInterval, T> map)
        {
            foreach (var i in intervals.ToIntervals())
                yield return new Interval<T>(i, map(i));
        }

        public static Interval<U> Map<T, U>(this IInterval<T> interval, Func<T, U> map)
        {
            var i = interval.ToInterval();
            return new Interval<U>(i, map(i.Value));
        }

        public static IEnumerable<Interval<U>> Map<T, U>(this IEnumerable<IInterval<T>> intervals, Func<T, U> map)
        {
            foreach (var i in intervals.ToIntervals())
                yield return new Interval<U>(i, map(i.Value));
        }

        public static IEnumerable<Interval<U>> Map<T, U>(this IEnumerable<IInterval<T>> intervals, Func<IInterval<T>, U> map)
        {
            foreach (var i in intervals.ToIntervals())
                yield return new Interval<U>(i, map(i));
        }

        //public static IEnumerable<Interval<string>> Slices(this IEnumerable<IInterval> intervals, string s)
        //{
        //    return intervals.Map(i => s.Substring(i));
        //}

        //public static IEnumerable<Interval<U>> Slices<T, U>(this IEnumerable<IInterval<T>> intervals, string s, Func<T, string, U> selector)
        //{
        //    return intervals.Map(i => selector(i.Value(), s.Substring(i)));
        //}

        public static IEnumerable<Interval<T[]>> Slices<T>(this IEnumerable<IInterval> intervals, IList<T> list)
        {
            return intervals.Map(i => i.Slice(list));
        }

        public static IEnumerable<Interval<T>> Inside<T>(this IEnumerable<IInterval<T>> intervals, IInterval range,
            IIntervalInclusionComparer comparer = null)
        {
            comparer = comparer ?? new LooseInclusionComparer();
            return intervals.ToIntervals().Where(i => comparer.Contains(range, i));
        }

        //public static IEnumerable<Interval<T>> Inside<T>(this IEnumerable<IInterval<T>> intervals, IInterval range,
        //    ContainsMode mode = ContainsMode.NON_STRICT)
        //{
        //    return intervals.ToIntervals().Where(i => range.Contains(i, mode));
        //}

        public static IEnumerable<Interval<T>> RemoveCoveredBy<T>(this IEnumerable<IInterval<T>> intervals,
            IEnumerable<IInterval<T>> otherSegments, ContainsMode mode = ContainsMode.NON_STRICT)
        {
            foreach (var interval in intervals.ToIntervals())
                if (!otherSegments.Any(i => i.Contains(interval, mode)))
                    yield return interval;
        }

        /// <summary>
        /// Returns non-empty intersections of the given range interval with every interval in the collection.
        /// </summary>
        public static IEnumerable<Interval<T>> IntersectWith<T>(this IEnumerable<IInterval<T>> intervals, IInterval range)
        {
            var r = range.ToInterval();
            foreach (var i in intervals.ToIntervals())
            {
                if (r.Contains(i))
                    yield return i;
                else if (r.Start <= i.Start && i.Start < r.End)
                    yield return new Interval<T>(i.Start, r.End - i.Start, i.Value);
                else if (r.Start < i.End && i.End <= r.End)
                    yield return new Interval<T>(r.Start, i.End - r.Start, i.Value);
                else if (i.Contains(r))
                    yield return new Interval<T>(r.Start, r.Length, i.Value);
            }
        }


        public static IEnumerable<Interval<T[]>> Intersections<T>(this IList<IInterval<T>> intervals)
        {
            var boundaries = intervals.Boundaries().ToList();
            if (boundaries.Count == 0)
                yield break;

            int minIdx = boundaries.First();
            int maxIdx = boundaries.Last();

            var currentGroup = new List<Interval<T>>();
            var groupStart = minIdx;

            foreach (var b in boundaries)
            {
                if (b > minIdx)
                {
                    if (currentGroup.Count > 0)
                        yield return new Interval<T[]>(groupStart, b - groupStart,
                            currentGroup.Select(s => s.Value).ToArray());
                    groupStart = b;
                }

                foreach (var s in intervals.ToIntervals())
                {
                    if (s.Start == b || (b == minIdx && s.Start <= b && s.End > b))
                        currentGroup.Add(s);
                    else if (s.End == b)
                        currentGroup.Remove(s);
                }
            }
        }

        public static IEnumerable<Interval<string>> Split(this IInterval<string> interval, char separator)
        {
            var i = interval.ToInterval();
            int pos = i.Start;
            foreach (var segment in i.Value.Split(separator))
            {
                yield return new Interval<string>(pos, segment.Length, segment);
                pos += segment.Length + 1;
            }
        }

        //public static IEnumerable<Interval<T>> GroupMerged<T>(this IEnumerable<IInterval<T>> intervals)
        //{
        //    foreach (var interval in intervals.Sorted())
        //    {

        //    }
        //}
    }

    //public class Alignment<T>
    //{
    //    public string Left { get; private set; }
    //    public string Right { get; private set; }
    //    public bool HasDifferences => Left != Right;
    //    public IntervalAlignment<T>[] Intervals { get; private set;}
    //    public IEnumerable<IInterval<T>> LeftIntervals => Intervals.Select(a => a.Left);
    //    public IEnumerable<IInterval<T>> RightIntervals => Intervals.Select(a => a.Right);
    //    public Dictionary<int, int> Mappings { get; private set; }

    //    public Alignment(string left, string right, IntervalAlignment<T>[] intervals, Dictionary<int, int> mappings)
    //    {
    //        Left = left;
    //        Right = right;
    //        Intervals = intervals;
    //        Mappings = mappings;
    //    }
    //}

    //public class Alignment
    //{
    //    public string From { get; private set; }
    //    public string To { get; private set; }
    //    public bool HasDifferences => From != To;
    //    public IntervalAlignment<string>[] Intervals { get; private set;}
    //    public IEnumerable<IInterval<string>> FromIntervals => Intervals.Select(a => a.Left);
    //    public IEnumerable<IInterval<string>> ToIntervals => Intervals.Select(a => a.Right);
    //    public Dictionary<int, int> Mappings { get; private set; }

    //    public Alignment(string from, string to, IntervalAlignment<string>[] intervals, Dictionary<int, int> mappings)
    //    {
    //        From = from;
    //        To = to;
    //        Intervals = intervals;
    //        Mappings = mappings;
    //    }
    //}

    //public class IntervalAlignment<T>
    //{
    //    public Interval<T> Left { get; private set; }
    //    public Interval<T> Right { get; private set; }

    //    public IntervalAlignment(Interval<T> from, Interval<T> to)
    //    {
    //        Left = from;
    //        Right = to;
    //    }
    //}

    //public class IntervalAlignment<T, U>
    //{
    //    public Interval<T> Left { get; private set; }
    //    public Interval<U> Right { get; private set; }

    //    public IntervalAlignment(Interval<T> left, Interval<U> right)
    //    {
    //        Left = left;
    //        Right = right;
    //    }
    //}
}
