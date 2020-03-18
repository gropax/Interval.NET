using Interval;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Intervals.Alignments
{
    public static class Alignment
    {
        public static Alignment<L, R> Create<L, R>(AlignedPair<L, R>[] alignedPairs)
        {
            var pairs = alignedPairs
                .Where(p => p.Left.Length + p.Right.Length > 0)
                .SqueezeReduce(
                    p => p.Left.Length == 0 ? -1 : p.Right.Length == 0 ? 1 : 0,
                    px => px.Concat());

            return new Alignment<L, R>(pairs.ToArray());
        }

        //public static Alignment<L, R> Create<L, R>(AlignedPair<L, R>[] alignedPairs)
        //{
        //    var pairs = new List<AlignedPair<L, R>>();

        //    AlignedPair<L, R> last = null;
        //    var group = new List<AlignedPair<L, R>>();
        //    int lastEmpty = 0;

        //    foreach (var pair in alignedPairs)
        //    {
        //        int pairEmpty = pair.Left.Length == 0 ? -1
        //            : pair.Right.Length == 0 ? 1
        //            : 0;

        //        if (last != null)
        //        {
        //            if (pairEmpty == 0)
        //            {
        //                if (group.Count > 0)
        //                {
        //                    pairs.Add(new AlignedPair<L, R>())
        //                }
        //            }

        //                group.Add(pair);
        //            else
        //        }

        //        last = pair;
        //        lastEmpty = pairEmpty;
        //    }
        //}
    }

    public class Alignment<L, R> : IEquatable<Alignment<L, R>>
    {
        public AlignedPair<L, R>[] AlignedPairs { get; }

        public Alignment(AlignedPair<L, R>[] alignedPairs)
        {
            AlignedPairs = alignedPairs;
        }

        public L[] GetLeft()
        {
            return AlignedPairs.SelectMany(p => p.Left).ToArray();
        }

        public R[] GetRight()
        {
            return AlignedPairs.SelectMany(p => p.Right).ToArray();
        }

        public Alignment<R, L> Reverse()
        {
            return new Alignment<R, L>(AlignedPairs.Select(p => p.Reverse()).ToArray());
        }

        public DetachedAlignment<L> DetachLeft()
        {
            var intervals = new List<Interval<L[]>>();

            int i = 0;
            foreach (var pair in AlignedPairs)
            {
                int l = pair.Right.Length;
                intervals.Add(new Interval<L[]>(i, l, pair.Left));
                i += l;
            }

            return new DetachedAlignment<L>(intervals.ToArray());
        }

        public DetachedAlignment<R> DetachRight()
        {
            var intervals = new List<Interval<R[]>>();

            int i = 0;
            foreach (var pair in AlignedPairs)
            {
                int l = pair.Left.Length;
                intervals.Add(new Interval<R[]>(i, l, pair.Right));
                i += l;
            }

            return new DetachedAlignment<R>(intervals.ToArray());
        }

        public bool Equals([AllowNull] Alignment<L, R> other)
        {
            if (other == null || AlignedPairs.Length != other.AlignedPairs.Length)
                return false;
            else
                return AlignedPairs.Zip(other.AlignedPairs, (s, o) => s.Equals(o)).All(b => b);
        }
    }



    public class AlignedPair<L, R> : IEquatable<AlignedPair<L, R>>
    {
        public L[] Left { get; }
        public R[] Right { get; }

        public AlignedPair(L[] left, R[] right)
        {
            Left = left;
            Right = right;
        }

        public AlignedPair<R, L> Reverse()
        {
            return new AlignedPair<R, L>(Right, Left);
        }

        public bool Equals([AllowNull] AlignedPair<L, R> other)
        {
            return Enumerable.SequenceEqual(Left, other.Left)
                && Enumerable.SequenceEqual(Right, other.Right);
        }
    }


    public static class AlignedPairExtensions
    {
        public static AlignedPair<L, R> Concat<L, R>(this IEnumerable<AlignedPair<L, R>> pairs)
        {
            var list = pairs.ToList();
            return new AlignedPair<L, R>(
                list.SelectMany(p => p.Left).ToArray(),
                list.SelectMany(p => p.Right).ToArray());
        }
    }
}
