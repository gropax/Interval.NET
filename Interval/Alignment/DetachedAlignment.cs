﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intervals.Alignments
{
    public static class DetachedAlignment
    {
        public static DetachedAlignment<char> Create(IEnumerable<IInterval<string>> intervals)
        {
            return Create(intervals.Select(i => i.Map(s => s.ToCharArray())));
        }

        public static DetachedAlignment<T> Create<T>(IEnumerable<IInterval<T[]>> intervals)
        {
            var sorted = new List<Interval<T[]>>();
            Interval<T[]> last = null;

            foreach (var interval in intervals.ToIntervals().OrderBy(i => i.Start))
            {
                if (last != null)
                {
                    if (last.End < interval.Start)
                        sorted.Add(new Interval<T[]>(last.End, interval.Start - last.End, new T[0]));
                    else if (last.End > interval.Start)
                        throw new DetachedAlignmentException($"Intervals [{last.Start}, {last.End}] and [{interval.Start}, {interval.End}] don't meet.");
                    else if (last.Length == 0 && interval.Length == 0)
                        throw new DetachedAlignmentException($"Consecutive zero-length intervals.");
                }

                sorted.Add(interval);
                last = interval;
            }

            return new DetachedAlignment<T>(sorted.ToArray());
        }
    }

    public class DetachedAlignment<T> : IInterval<T[]>
    {
        public Interval<T[]>[] Intervals { get; }
        public int Start { get; }
        public int Length { get; }

        internal DetachedAlignment(Interval<T[]>[] intervals)
        {
            Intervals = intervals;
            Start = intervals[0].Start;
            Length = intervals[^1].End - Start;
        }

        public Interval<T[]> ToInterval()
        {
            return new Interval<T[]>(Start, Length, GetValue());
        }

        Interval IInterval.ToInterval()
        {
            return new Interval(Start, Length);
        }

        public T[] GetValue()
        {
            return Intervals.Values().SelectMany(v => v).ToArray();
        }


        public DetachedAlignment<T> Concat(DetachedAlignment<T> other)
        {
            var last = Intervals[^1];
            var otherFirst = other.Intervals[0];

            bool consecutiveZeroLength = last.Length == 0 && otherFirst.Length == 0;
            var intervals = new List<Interval<T[]>>(Intervals);

            if (last.End > otherFirst.Start)
                throw new DetachedAlignmentException($"Intervals [{last.Start}, {last.End}] and [{otherFirst.Start}, {otherFirst.End}] don't meet.");
            else if (last.End < otherFirst.Start)
                intervals.Add(new Interval<T[]>(last.End, otherFirst.Start - last.End, new T[0]));
            else if (consecutiveZeroLength)
                intervals.Add(new Interval<T[]>(last.Start, 0, last.Value.Concat(otherFirst.Value).ToArray()));

            if (consecutiveZeroLength)
                intervals.AddRange(other.Intervals.Skip(1));
            else
                intervals.AddRange(other.Intervals);

            return new DetachedAlignment<T>(intervals.ToArray());
        }


        public Alignment<T, R> AttachLeft<R>(R[] right)
        {
            ValidateLength(right);
            return new Alignment<T, R>(Intervals.Select(i =>
                new AlignedPair<T, R>(i.Value, i.Slice(right))).ToArray());
        }

        public Alignment<L, T> AttachRight<L>(L[] left)
        {
            ValidateLength(left);
            return new Alignment<L, T>(Intervals.Select(i =>
                new AlignedPair<L, T>(i.Slice(left), i.Value)).ToArray());
        }

    //public interface ISequentiallyOrdered<T>
    //{
    //    Interval<T>[] Intevals { get; }
    //}
        //public Sequential.SortedIntervals<Interval<T>> ValueIntervals()
        //{
        //    var valueIntervals = new List<Interval<Interval<T>>>();
        //    int index = 0;

        //    foreach (var interval in Intervals)
        //    {
        //        valueIntervals.Add(new Interval<T>)
        //    }

        //    Intervals.Select(i => new Interval<Interval<T>>(i.Start + acc, ))
        //}

        //public DetachedAlignment<T> Compose<U>(DetachedAlignment<U> alignment)
        //{
        //    ValidateLength(alignment.Value);
        //}

        //private (Interval<T>[], Interval<U>[]) SmallestAlignedIntervals<U>(
        //    DetachedAlignment<U> alignment, int index)
        //{
        //    var leftInterval = Intervals[index];
        //    var leftRange = leftInterval;

        //    while (true)
        //    {
        //        var rightIntervals = alignment.Intervals.IntersectingWith(leftRange);
        //        var leftIntervals = Intervals.Covering
        //    }
        //}

        private void ValidateLength<X>(X[] xs)
        {
            if (xs.Length != Length)
                throw new DetachedAlignmentException("Length mismatch.");
        }
    }



    [Serializable]
    public class DetachedAlignmentException : Exception
    {
        public DetachedAlignmentException(string message)
            : base(message) { }
    }
}
