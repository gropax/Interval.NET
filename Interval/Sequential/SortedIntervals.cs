using Interval;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intervals.Sequential
{
    public class SortedIntervals<T> : IEnumerable<Interval<T>>
    {
        public Interval<T>[] Intervals { get; }

        public SortedIntervals(Interval<T>[] intervals)
        {
            Intervals = intervals;
        }

        public IEnumerator<Interval<T>> GetEnumerator()
        {
            return Intervals.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Interval<T> this[Index index] => Intervals[index];
    }


    public static class SortedIntervalsExtensions
    {
        public static bool HasOverlaps<T>(this SortedIntervals<T> intervals)
        {
            Interval<T> last = null;

            foreach (var interval in intervals)
            {
                if (last != null && last.End > interval.Start)
                    return true;
                last = interval;
            }

            return false;
        }

        //public static SortedIntervals<U> Map<T, U>(this SortedIntervals<T> intervals,
        //    Func<IEnumerable<IInterval<T>>, Interval<U>[]> selector)
        //{
        //    return new SortedIntervals<U>(selector(intervals.Intervals).ToArray());
        //}

        public static SortedIntervals<T> Complete<T>(this SortedIntervals<T> intervals,
            Func<Interval, T> selector)
        {
            return new SortedIntervals<T>(CompleteEnum(intervals, selector).ToArray());
        }

        static IEnumerable<Interval<T>> CompleteEnum<T>(this SortedIntervals<T> intervals,
            Func<Interval, T> selector)
        {
            Interval<T> last = null;
            foreach (var interval in intervals)
            {
                if (last != null && last.End < interval.Start)
                {
                    var i = new Interval(last.End, interval.Start - last.End);
                    yield return new Interval<T>(i, selector(i));
                }
                yield return interval;
                last = interval;
            }
        }

        public static SortedIntervals<U> SqueezeReduce<T, U, X>(this SortedIntervals<T> intervals,
            Func<Interval<T>, X> selector, Func<Interval<T>[], Interval<U>> reducer)
        {
            return new SortedIntervals<U>(intervals.Intervals.SqueezeReduce(selector, reducer).ToArray());
        }
    }
}
