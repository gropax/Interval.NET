using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interval.Alignment
{
    public class Alignment<L, R>
    {
        public AlignedPair<L, R>[] AlignedPairs { get; }
        public L[] Left => AlignedPairs.SelectMany(p => p.Left).ToArray();
        public R[] Right => AlignedPairs.SelectMany(p => p.Right).ToArray();

        public Alignment(AlignedPair<L, R>[] alignedPairs)
        {
            AlignedPairs = alignedPairs;
        }

        public Alignment<R, L> Inverse()
        {
            return new Alignment<R, L>(AlignedPairs.Select(p => p.Inverse()).ToArray());
        }

        //public DetachedAlignment<L> DetachLeft()
        //{
        //    var intervals = new List<Interval<L[]>>();

        //    int i = 0;
        //    foreach (var pair in AlignedPairs)
        //    {
        //        int l = pair.Right.Length;
        //        intervals.Add(new Interval<L[]>(i, l, pair.Left));
        //        i += l;
        //    }

        //    return new DetachedAlignment<L>(intervals.ToArray());
        //}

        //public DetachedAlignment<R> DetachRight()
        //{
        //    var intervals = new List<Interval<R[]>>();

        //    int i = 0;
        //    foreach (var pair in AlignedPairs)
        //    {
        //        int l = pair.Left.Length;
        //        intervals.Add(new Interval<R[]>(i, l, pair.Right));
        //        i += l;
        //    }

        //    return new DetachedAlignment<R>(intervals.ToArray());
        //}
    }



    public class AlignedPair<L, R>
    {
        public L[] Left { get; }
        public R[] Right { get; }

        public AlignedPair(L[] left, R[] right)
        {
            Left = left;
            Right = right;
        }

        public AlignedPair<R, L> Inverse()
        {
            return new AlignedPair<R, L>(Right, Left);
        }
    }
}
