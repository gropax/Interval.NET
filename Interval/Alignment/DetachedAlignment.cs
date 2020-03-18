using Intervals.Sequential;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly:InternalsVisibleTo("Interval.Tests")]
namespace Intervals.Alignments
{
    public static class DetachedAlignment
    {
        public static DetachedAlignment<T> Create<T>(IEnumerable<IInterval<T[]>> intervals)
        {
            return Create(intervals.SortedSequentially());
        }

        public static DetachedAlignment<char> Create(IEnumerable<IInterval<string>> intervals)
        {
            return Create(intervals.Select(i => i.Map(s => s.ToCharArray())));
        }

        public static DetachedAlignment<T> Create<T>(Sequential.SortedIntervals<T[]> intervals)
        {
            if (intervals.HasOverlaps())
                throw new DetachedAlignmentException($"Interval overlapping.");
            else
                return new DetachedAlignment<T>(intervals
                    .Complete(_ => new T[0])
                    .SqueezeReduce(
                        i => i.Value.Length == 0 ? 1 : i.Length == 0 ? -1 : 0,
                        ix => new Interval<T[]>(ix.Range<T[]>(), ix.SelectMany(i => i.Value).ToArray()))
                    .Intervals);
        }

        //public static DetachedAlignment<char> Create(IEnumerable<IInterval<string>> intervals)
        //{
        //    return Create(intervals.Select(i => i.Map(s => s.ToCharArray())));
        //}

        //public static DetachedAlignment<T> Create<T>(IEnumerable<IInterval<T[]>> intervals)
        //{
        //    var sorted = new List<Interval<T[]>>();
        //    Interval<T[]> last = null;

        //    foreach (var interval in intervals.ToIntervals().OrderBy(i => i.Start).ThenBy(i => i.Length))
        //    {
        //        if (last != null)
        //        {
        //            if (last.End < interval.Start)
        //                sorted.Add(new Interval<T[]>(last.End, interval.Start - last.End, new T[0]));
        //            else if (last.End > interval.Start)
        //                throw new DetachedAlignmentException($"Intervals [{last.Start}, {last.End}] and [{interval.Start}, {interval.End}] don't meet.");
        //            else if (last.Length == 0 && interval.Length == 0)
        //                throw new DetachedAlignmentException($"Consecutive zero-length intervals.");
        //        }

        //        sorted.Add(interval);
        //        last = interval;
        //    }

        //    return new DetachedAlignment<T>(sorted.ToArray());
        //}
    }

    public class DetachedAlignment<T> : IInterval<T[]>, IEquatable<DetachedAlignment<T>>
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


        public DetachedAlignment<T> Join(DetachedAlignment<T> other)
        {
            if (Intervals[^1].End > other.Intervals[0].Start)
                throw new DetachedAlignmentException($"Interval overlapping.");
            else
            {
                var sorted = new SortedIntervals<T[]>(Intervals.Concat(other.Intervals).ToArray());
                return new DetachedAlignment<T>(sorted
                    .Complete(_ => new T[0])
                    .SqueezeReduce(
                        i => i.Value.Length == 0 ? 1 : i.Length == 0 ? -1 : 0,
                        ix => new Interval<T[]>(ix.Range<T[]>(), ix.SelectMany(i => i.Value).ToArray()))
                    .Intervals);
            }
        }


        public Alignment<T, R> AttachLeft<R>(R[] right)
        {
            ValidateLength(right);
            return new Alignment<T, R>(Intervals.Select(i =>
                new AlignedPair<T, R>(i.Value, right[i])).ToArray());
        }

        public Alignment<L, T> AttachRight<L>(L[] left)
        {
            ValidateLength(left);
            return new Alignment<L, T>(Intervals.Select(i =>
                new AlignedPair<L, T>(i.Slice(left), i.Value)).ToArray());
        }

        private void ValidateLength<X>(X[] xs)
        {
            int length = xs.Length;
            int end = Start + Length;
            if (Start < 0 || Start > length || end < 0 || end > length)
                throw new DetachedAlignmentException("Length mismatch.");
        }

        public bool Equals([AllowNull] DetachedAlignment<T> other)
        {
            if (other == null || Intervals.Length != other.Intervals.Length)
                return false;
            else
                return Intervals.Zip(other.Intervals, (s, o) => s.Equals(o)).All(b => b);
        }

        public DetachedAlignment<T> Translate(int value)
        {
            return new DetachedAlignment<T>(Intervals.Translate(value).ToArray());
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
    }



    [Serializable]
    public class DetachedAlignmentException : Exception
    {
        public DetachedAlignmentException(string message)
            : base(message) { }
    }
}
