using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interval
{
    public interface IInterval
    {
        Interval ToInterval();
    }

    public class Interval : IInterval, IEquatable<IInterval>, IComparable<IInterval>
    {
        public int Start { get; private set; }
        public int Length { get; private set; }

        public int End { get; }
        public int[] Boundaries { get { return new int[] { Start, End }; } }

        public Interval(Interval interval)
        {
            Start = interval.Start;
            Length = interval.Length;
            End = interval.End;
        }

        public Interval(int start, int length)
        {
            if (length < 0)
                throw new ArgumentException("Interval length can't be less than 0.");

            Start = start;
            Length = length;
            End = start + length;
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

        //public static bool operator ==(Interval i1, Interval i2)
        //{
        //    return i1.Equals(i2);
        //}

        //public static bool operator !=(Interval i1, Interval i2)
        //{
        //    return !i1.Equals(i2);
        //}

        public int CompareTo(IInterval other)
        {
            if (other == null) return 1;
            var i = other.ToInterval();
            var startCmp = Start.CompareTo(i.Start);
            return startCmp != 0 ? startCmp : i.End.CompareTo(End);
        }

        public static bool operator <(Interval i1, Interval i2)
        {
            return i1.CompareTo(i2) == -1;
        }

        public static bool operator >(Interval i1, Interval i2)
        {
            return i1.CompareTo(i2) == 1;
        }

        public static bool operator <=(Interval i1, Interval i2)
        {
            return i1.CompareTo(i2) <= 0;
        }

        public static bool operator >=(Interval i1, Interval i2)
        {
            return i1.CompareTo(i2) >= 0;
        }
    }


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
            return new HashSet<int>(intervals.ToIntervals().SelectMany(i => i.Boundaries)).OrderBy(b => b).ToArray();
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



    //public interface IInterval
    //{
    //    int Start { get; }
    //    int Length { get; }
    //    int End { get; }
    //    int[] Boundaries { get; }
    //}

    //public class Interval : IInterval
    //{
    //    public int Start { get; private set; }
    //    public int Length { get; private set; }
    //    public int End { get; }
    //    public int[] Boundaries { get { return new int[] { Start, End }; } }

    //    public Interval(IInterval interval)
    //    {
    //        Start = interval.Start;
    //        Length = interval.Length;
    //        End = interval.End;
    //    }

    //    public Interval(int start, int length)
    //    {
    //        if (length < 0)
    //            throw new ArgumentException("Interval length can't be less than 0.");

    //        Start = start;
    //        Length = length;
    //        End = start + length;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        var other = obj as Interval;
    //        if (other == null)
    //            return false;

    //        return Start == other.Start && Length == other.Length;
    //    }

    //    public override int GetHashCode()
    //    {
    //        unchecked
    //        {
    //            return 17 * Start.GetHashCode() + 19 * End.GetHashCode();
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return $"Interval([{Start}, {End}])";
    //    }
    //}

    //public class Interval<T> : Interval, IInterval<T>
    //{
    //    public T Content { get; private set; }

    //    public Interval(IInterval interval, T content) : base (interval)
    //    {
    //        Content = content;
    //    }

    //    public Interval(int start, int length, T content) : base (start, length)
    //    {
    //        Content = content;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        var other = obj as Interval<T>;
    //        if (other == null)
    //            return false;

    //        var equality = Start == other.Start && Length == other.Length &&
    //            Content.Equals(other.Content);

    //        return equality;
    //    }

    //    public override int GetHashCode()
    //    {
    //        unchecked
    //        {
    //            return 17 * Start.GetHashCode() + 19 * End.GetHashCode() + 23 * Content.GetHashCode();
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return $"Interval([{Start}, {End}], Content = {Content})";
    //    }
    //}

    //public enum ContainsMode
    //{
    //    STRICT,
    //    NON_STRICT,
    //}

    //public static class IntervalExtensions
    //{
    //    public static bool IsEmpty(this IInterval self)
    //    {
    //        return self.Length == 0;
    //    }

    //    public static Interval Forget<T>(this IInterval<T> i)
    //    {
    //        return new Interval(i.Start, i.Length);
    //    }


    //    public static bool IsBefore(this IInterval self, IInterval other,
    //        ContainsMode mode = ContainsMode.NON_STRICT)
    //    {
    //        if (mode == ContainsMode.NON_STRICT)
    //            return self.End <= other.Start;
    //        else
    //            return self.End < other.Start;
    //    }

    //    public static bool IsAfter(this IInterval self, IInterval other,
    //        ContainsMode mode = ContainsMode.NON_STRICT)
    //    {
    //        if (mode == ContainsMode.NON_STRICT)
    //            return self.Start >= other.End;
    //        else
    //            return self.Start > other.End;
    //    }

    //    public static bool Meets(this IInterval self, IInterval other)
    //    {
    //        return self.End == other.Start;
    //    }

    //    //public static bool Overlaps(this ISegment self, ISegment other)
    //    //{
    //    //    return self.Start < other.Start && other.Start < self.End;
    //    //}


    //    public static bool Starts(this IInterval self, IInterval other)
    //    {
    //        return self.Start == other.Start && self.End < other.End;
    //    }

    //    public static bool Finishes(this IInterval self, IInterval other)
    //    {
    //        return self.Start > other.Start && self.End == other.End;
    //    }

    //    public static bool IsMergedWith(this IInterval self, IInterval other)
    //    {
    //        return self.Start == other.Start && other.End == self.End;
    //    }

    //    public static bool Contains(this IInterval self, IInterval other,
    //        ContainsMode mode = ContainsMode.NON_STRICT)
    //    {
    //        if (mode == ContainsMode.NON_STRICT)
    //            return self.Start <= other.Start && other.End <= self.End;
    //        else
    //            return self.Start < other.Start && other.End < self.End;
    //    }

    //    public static bool Contains(this IInterval self, int index,
    //        ContainsMode mode = ContainsMode.NON_STRICT)
    //    {
    //        if (mode == ContainsMode.NON_STRICT)
    //            return self.Start <= index && index <= self.End;
    //        else
    //            return self.Start < index && index < self.End;
    //    }

    //    public static IInterval Range(this IList<IInterval> intervals)
    //    {
    //        var start = intervals.Min(i => i.Start);
    //        var end = intervals.Max(i => i.End);
    //        return new Interval(start, end - start);
    //    }

    //    public static Interval<T[]> Range<T>(this IList<IInterval<T>> intervals)
    //    {
    //        var start = intervals.Min(i => i.Start);
    //        var end = intervals.Max(i => i.End);
    //        return new Interval<T[]>(start, end - start, intervals.Select(i => i.Content).ToArray());
    //    }

    //    public static Interval<U> Range<T, U>(this IList<IInterval<T>> intervals, Func<IEnumerable<T>, U> reduce)
    //    {
    //        var start = intervals.Min(i => i.Start);
    //        var end = intervals.Max(i => i.End);
    //        return new Interval<U>(start, end - start, reduce(intervals.Select(i => i.Content)));
    //    }



    //    public static IEnumerable<int> Boundaries(this IEnumerable<IInterval> intervals)
    //    {
    //        return new HashSet<int>(intervals.SelectMany(i => i.Boundaries)).OrderBy(b => b);
    //    }

    //    public static T[] Slice<T>(this IInterval i, IEnumerable<T> list)
    //    {
    //        return list.Skip(i.Start).Take(i.Length).ToArray();
    //    }

    //    public static string Slice(this IInterval i, string s)
    //    {
    //        return s.Substring(i.Start, i.Length);
    //    }

    //    public static IEnumerable<IInterval<T>> Sorted<T>(this IEnumerable<IInterval<T>> intervals)
    //    {
    //        return intervals.OrderBy(i => i.Start).ThenByDescending(i => i.Length);
    //    }

    //    public static IEnumerable<Interval<T>> Map<T>(this IEnumerable<IInterval> intervals, Func<IInterval, T> map)
    //    {
    //        foreach (var i in intervals)
    //            yield return new Interval<T>(i, map(i));
    //    }

    //    public static IEnumerable<Interval<U>> Map<T, U>(this IEnumerable<IInterval<T>> intervals, Func<T, U> map)
    //    {
    //        foreach (var i in intervals)
    //            yield return new Interval<U>(i, map(i.Content));
    //    }

    //    public static IEnumerable<Interval<U>> Map<T, U>(this IEnumerable<IInterval<T>> intervals, Func<IInterval<T>, U> map)
    //    {
    //        foreach (var i in intervals)
    //            yield return new Interval<U>(i, map(i));
    //    }

    //    public static IEnumerable<Interval<string>> Slices(this IEnumerable<IInterval> intervals, string s)
    //    {
    //        return intervals.Map(i => s.Substring(i));
    //    }

    //    public static IEnumerable<Interval<T[]>> Slices<T>(this IEnumerable<IInterval> intervals, IList<T> list)
    //    {
    //        return intervals.Map(i => i.Slice(list));
    //    }

    //    public static IEnumerable<IInterval<T>> Inside<T>(this IEnumerable<IInterval<T>> intervals, IInterval range,
    //        ContainsMode mode = ContainsMode.NON_STRICT)
    //    {
    //        return intervals.Where(i => range.Contains(i, mode));
    //    }

    //    public static IEnumerable<IInterval<T>> RemoveCoveredBy<T>(this IEnumerable<IInterval<T>> intervals,
    //        IEnumerable<IInterval<T>> otherSegments, ContainsMode mode = ContainsMode.NON_STRICT)
    //    {
    //        foreach (var interval in intervals)
    //            if (!otherSegments.Any(i => i.Contains(interval, mode)))
    //                yield return interval;
    //    }

    //    /// <summary>
    //    /// Filter out the intervals that are entirely covered by another interval.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="intervals"></param>
    //    /// <returns></returns>
    //    public static IEnumerable<IInterval<T>> RemoveCovered<T>(this IEnumerable<IInterval<T>> intervals,
    //        ContainsMode mode = ContainsMode.NON_STRICT)
    //    {
    //        IInterval<T> current = null;
    //        foreach (var interval in intervals.Sorted())
    //        {
    //            if (current == null)
    //                current = interval;
    //            else if (!current.Contains(interval, mode))
    //            {
    //                yield return interval;
    //                current = interval;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Filter out the intervals that are entirely covered by another interval, if provided
    //    /// condition is satisfied. The condition's first parameter is the covering interval,
    //    /// the second one is the covered interval. If the condition function returns `true` the
    //    /// covered interval will be removed.
    //    /// </summary>
    //    public static IEnumerable<IInterval<T>> RemoveCovered<T>(this IEnumerable<IInterval<T>> intervals,
    //        Func<T, T, bool> condition, ContainsMode mode = ContainsMode.NON_STRICT)
    //    {
    //        IInterval<T> current = null;
    //        foreach (var interval in intervals.Sorted())
    //        {
    //            if (current == null)
    //                current = interval;
    //            else if (!current.Contains(interval, mode) || !condition(current.Content, interval.Content))
    //            {
    //                yield return interval;
    //                current = interval;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Returns non-empty intersections of the given range interval with every interval in the collection.
    //    /// </summary>
    //    public static IEnumerable<IInterval<T>> IntersectWith<T>(this IEnumerable<IInterval<T>> intervals, IInterval range)
    //    {
    //        foreach (var i in intervals)
    //        {
    //            if (range.Contains(i))
    //                yield return i;
    //            else if (range.Start <= i.Start && i.Start < range.End)
    //                yield return new Interval<T>(i.Start, range.End - i.Start, i.Content);
    //            else if (range.Start < i.End && i.End <= range.End)
    //                yield return new Interval<T>(range.Start, i.End - range.Start, i.Content);
    //            else if (i.Contains(range))
    //                yield return new Interval<T>(range.Start, range.Length, i.Content);
    //        }
    //    }



    //    public static IEnumerable<IInterval<T[]>> Union<T>(this IEnumerable<IInterval<T>> intervals)
    //    {
    //        return intervals.UnionBase().Select(g => g.Range());
    //    }

    //    public static IEnumerable<IInterval<U>> Union<T, U>(this IEnumerable<IInterval<T>> intervals, Func<IEnumerable<T>, U> reduce)
    //    {
    //        return intervals.UnionBase().Select(g => g.Range(reduce));
    //    }

    //    private static IEnumerable<List<IInterval<T>>> UnionBase<T>(this IEnumerable<IInterval<T>> intervals)
    //    {
    //        var currentGroup = new List<IInterval<T>>();
    //        IInterval<T> lastInterval = null;
    //        foreach (var interval in intervals.Sorted())
    //        {
    //            if (lastInterval == null || interval.Start <= lastInterval.End)
    //                currentGroup.Add(interval);
    //            else
    //            {
    //                yield return currentGroup;
    //                currentGroup = new List<IInterval<T>>() { interval };
    //            }
    //            lastInterval = interval;
    //        }

    //        if (currentGroup.Count > 0)
    //            yield return currentGroup;
    //    }


    //    public static IEnumerable<Interval<T[]>> Intersections<T>(this IList<IInterval<T>> intervals)
    //    {
    //        var boundaries = intervals.Boundaries().ToList();
    //        if (boundaries.Count == 0)
    //            yield break;

    //        int minIdx = boundaries.First();
    //        int maxIdx = boundaries.Last();

    //        var currentGroup = new List<IInterval<T>>();
    //        var groupStart = minIdx;

    //        foreach (var b in boundaries)
    //        {
    //            if (b > minIdx)
    //            {
    //                if (currentGroup.Count > 0)
    //                    yield return new Interval<T[]>(groupStart, b - groupStart,
    //                        currentGroup.Select(s => s.Content).ToArray());
    //                groupStart = b;
    //            }

    //            foreach (var s in intervals)
    //            {
    //                if (s.Start == b || (b == minIdx && s.Start <= b && s.End > b))
    //                    currentGroup.Add(s);
    //                else if (s.End == b)
    //                    currentGroup.Remove(s);
    //            }
    //        }
    //    }

    //    public static IEnumerable<Interval<string>> Split(this IInterval<string> interval, char separator)
    //    {
    //        int i = interval.Start;
    //        foreach (var segment in interval.Content.Split(separator))
    //        {
    //            yield return new Interval<string>(i, segment.Length, segment);
    //            i += segment.Length + 1;
    //        }
    //    }
    //}

    //public class Alignment
    //{
    //    public string From { get; private set; }
    //    public string To { get; private set; }
    //    public bool HasDifferences => From != To;
    //    public IntervalAlignment<string>[] Intervals { get; private set;}
    //    public IEnumerable<IInterval<string>> FromIntervals => Intervals.Select(a => a.From);
    //    public IEnumerable<IInterval<string>> ToIntervals => Intervals.Select(a => a.To);
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
    //    public IInterval<T> From { get; private set; }
    //    public IInterval<T> To { get; private set; }

    //    public IntervalAlignment(IInterval<T> from, IInterval<T> to)
    //    {
    //        From = from;
    //        To = to;
    //    }
    //}

    ////public static class AlignmentExtensions
    ////{
    ////    public static IInterval<U> Map<T, U>(this Alignment alignment, IInterval<T> interval,
    ////        Func<IInterval<T>, U> map)
    ////    {
    ////        var start = alignment.Mappings[interval.Start];
    ////        var end = alignment.Mappings[interval.End];
    ////        return new Interval<U>(start, end - start, map(interval));
    ////    }
    ////}
}
