using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interval.Sequential
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
}
