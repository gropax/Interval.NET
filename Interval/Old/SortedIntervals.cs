using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interval
{
    public class SortedIntervals : IEnumerable<Interval>
    {
        private readonly IEnumerable<Interval> _intervals;

        public SortedIntervals(IEnumerable<Interval> intervals)
        {
            _intervals = intervals;
        }

        public IEnumerator<Interval> GetEnumerator()
        {
            return _intervals.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class SortedIntervals<T> : IEnumerable<Interval<T>>
    {
        private readonly IEnumerable<Interval<T>> _intervals;

        public SortedIntervals(IEnumerable<Interval<T>> intervals)
        {
            _intervals = intervals;
        }

        public IEnumerator<Interval<T>> GetEnumerator()
        {
            return _intervals.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    public static class SortedIntervalsExtensions
    {
        public static SortedIntervals<T> AssumeSorted<T>(this IEnumerable<IInterval<T>> intervals)
        {
            return new SortedIntervals<T>(intervals.ToIntervals().ToList());
        }

        public static SortedIntervals<U> Map<T, U>(this SortedIntervals<T> intervals, Func<T, U> selector)
        {
            return new SortedIntervals<U>(intervals.Map(selector));
        }

        public static SortedIntervals<T> Translate<T>(this SortedIntervals<T> intervals, int value)
        {
            return new SortedIntervals<T>(intervals.AsEnumerable().Translate(value));
        }

        public static SortedIntervals<T> Inside<T>(this SortedIntervals<T> intervals, IInterval range,
            IIntervalInclusionComparer comparer = null)
        {
            return new SortedIntervals<T>(intervals.AsEnumerable().Inside(range, comparer));
        }

        /// <summary>
        /// Filter out the intervals that are entirely covered by another interval.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="intervals"></param>
        /// <returns></returns>
        public static SortedIntervals<T> RemoveCovered<T>(this SortedIntervals<T> intervals,
            ContainsMode mode = ContainsMode.NON_STRICT)
        {
            return new SortedIntervals<T>(RemoveCoveredEnumerator(intervals, mode));
        }

        private static IEnumerable<Interval<T>> RemoveCoveredEnumerator<T>(this SortedIntervals<T> intervals,
            ContainsMode mode = ContainsMode.NON_STRICT)
        {
            IInterval<T> current = null;
            foreach (var interval in intervals)
            {
                if (current == null)
                    current = interval;
                else if (!current.Contains(interval, mode))
                {
                    yield return interval;
                    current = interval;
                }
            }
        }

        /// <summary>
        /// Filter out the intervals that are entirely covered by another interval, if provided
        /// condition is satisfied. The condition's first parameter is the covering interval,
        /// the second one is the covered interval. If the condition function returns `true` the
        /// covered interval will be removed.
        /// </summary>
        public static SortedIntervals<T> RemoveCovered<T>(this SortedIntervals<T> intervals,
            Func<T, T, bool> condition, ContainsMode mode = ContainsMode.NON_STRICT)
        {
            return new SortedIntervals<T>(RemoveCoveredEnumerator(intervals, condition, mode));
        }

        private static IEnumerable<Interval<T>> RemoveCoveredEnumerator<T>(this SortedIntervals<T> intervals,
            Func<T, T, bool> condition, ContainsMode mode = ContainsMode.NON_STRICT)
        {
            Interval<T> current = null;
            foreach (var interval in intervals)
            {
                if (current == null)
                    current = interval;
                else if (!current.Contains(interval, mode) || !condition(current.Value, interval.Value))
                {
                    yield return interval;
                    current = interval;
                }
            }
        }

        public static SortedIntervals<T[]> Union<T>(this SortedIntervals<T> intervals)
        {
            return new SortedIntervals<T[]>(intervals.UnionEnumerator().Select(g => g.Range()));
        }

        public static SortedIntervals<U> Union<T, U>(this SortedIntervals<T> intervals, Func<IEnumerable<T>, U> reduce)
        {
            return new SortedIntervals<U>(intervals.UnionEnumerator().Select(g => g.Range(reduce)));
        }

        private static IEnumerable<List<IInterval<T>>> UnionEnumerator<T>(this SortedIntervals<T> intervals)
        {
            var currentGroup = new List<IInterval<T>>();
            int lastEnd = int.MinValue;

            foreach (var interval in intervals)
            {
                if (lastEnd == int.MinValue || interval.Start <= lastEnd)
                    currentGroup.Add(interval);
                else
                {
                    yield return currentGroup;
                    currentGroup = new List<IInterval<T>>() { interval };
                }
                lastEnd = Math.Max(lastEnd, interval.End);
            }

            if (currentGroup.Count > 0)
                yield return currentGroup;
        }

        public static SortedIntervals<T> IntersectWith<T>(this SortedIntervals<T> intervals, IInterval range)
        {
            return new SortedIntervals<T>(intervals.AsEnumerable().IntersectWith(range));
        }

        public static SortedIntervals<T[]> GroupMerged<T>(this SortedIntervals<T> intervals)
        {
            return new SortedIntervals<T[]>(GroupMergedEnumerator(intervals));
        }

        public static IEnumerable<Interval<T[]>> GroupMergedEnumerator<T>(this SortedIntervals<T> intervals)
        {
            var group = new List<Interval<T>>();

            foreach (var interval in intervals)
            {
                if (group.Count > 0 && !group[0].Equals(interval))
                {
                    yield return new Interval<T[]>(group[0], group.Values().ToArray());
                    group = new List<Interval<T>>();
                }
                group.Add(interval);
            }

            if (group.Count > 0)
                yield return new Interval<T[]>(group[0], group.Values().ToArray());
        }

        public static SortedIntervals<TResult> Align<TFirst, TSecond, TResult>(
            this SortedIntervals<TFirst> intervals, SortedIntervals<TSecond> otherIntervals,
            Func<TFirst[], TSecond[], TResult> selector)
        {
            return new AlignSortedIntervals<TFirst, TSecond, TResult>(intervals, otherIntervals, selector).Execute();
        }
    }
}
