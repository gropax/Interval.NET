using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interval
{
    public class AlignSortedIntervals<TFirst, TSecond, TResult>
    {
        private readonly SortedIntervals<TFirst> _firstIntervals;
        private readonly SortedIntervals<TSecond> _secondIntervals;
        private readonly Func<TFirst[], TSecond[], TResult> _selector;

        private IEnumerator<Interval<TFirst[]>> _firstEnumerator;
        private IEnumerator<Interval<TSecond[]>> _secondEnumerator;

        public AlignSortedIntervals(SortedIntervals<TFirst> firstIntervals,
            SortedIntervals<TSecond> secondIntervals, Func<TFirst[], TSecond[], TResult> selector)
        {
            _firstIntervals = firstIntervals;
            _secondIntervals = secondIntervals;
            _selector = selector;
        }

        public SortedIntervals<TResult> Execute()
        {
            return new SortedIntervals<TResult>(AlignEnumerator());
        }

        private IEnumerable<Interval<TResult>> AlignEnumerator()
        {
            _firstEnumerator = _firstIntervals.GroupMergedEnumerator().GetEnumerator();
            _secondEnumerator = _secondIntervals.GroupMergedEnumerator().GetEnumerator();

            Interval<TFirst[]> fst;
            Interval<TSecond[]> snd;

            #region Pick first or finalize second
            if (!TryPick(_firstEnumerator, out fst))
            {
                foreach (var interval in FinalizeSecond())
                    yield return interval;
                yield break;
            }
            #endregion
            #region Pick second or finalize first
            if (!TryPick(_secondEnumerator, out snd))
            {
                foreach (var interval in FinalizeFirst())
                    yield return interval;
                yield break;
            }
            #endregion

            while (true)
            {
                var cmp = fst.CompareTo(snd);
                if (cmp == -1)
                {   // fst interval in before snd: yield fst
                    yield return new Interval<TResult>(fst, _selector(fst.Value, new TSecond[0]));
                    #region Pick first or finalize second
                    if (!TryPick(_firstEnumerator, out fst))
                    {
                        foreach (var interval in FinalizeSecond())
                            yield return interval;
                        yield break;
                    }
                    #endregion
                }
                else if (cmp == 1)
                {   // fst interval in after snd: yield snd
                    yield return new Interval<TResult>(snd, _selector(new TFirst[0], snd.Value));
                    #region Pick second or finalize first
                    if (!TryPick(_secondEnumerator, out snd))
                    {
                        foreach (var interval in FinalizeFirst())
                            yield return interval;
                        yield break;
                    }
                    #endregion
                }
                else
                {   // fst and snd intervals are merged: yield both
                    yield return new Interval<TResult>(fst, _selector(fst.Value, snd.Value));
                    #region Pick first or finalize second
                    if (!TryPick(_firstEnumerator, out fst))
                    {
                        _secondEnumerator.MoveNext();  // current snd has been yielded already
                        foreach (var interval in FinalizeSecond())
                            yield return interval;
                        yield break;
                    }
                    #endregion
                    #region Pick second or finalize first
                    if (!TryPick(_secondEnumerator, out snd))
                    {
                        foreach (var interval in FinalizeFirst())
                            yield return interval;
                        yield break;
                    }
                    #endregion
                }
            }
        }

        private IEnumerable<Interval<TResult>> FinalizeFirst()
        {
            if (_firstEnumerator.Current != null)
                yield return new Interval<TResult>(_firstEnumerator.Current, _selector(_firstEnumerator.Current.Value, new TSecond[0]));

            while (_firstEnumerator.MoveNext())
                yield return new Interval<TResult>(_firstEnumerator.Current, _selector(_firstEnumerator.Current.Value, new TSecond[0]));
        }

        private IEnumerable<Interval<TResult>> FinalizeSecond()
        {
            if (_secondEnumerator.Current != null)
                yield return new Interval<TResult>(_secondEnumerator.Current, _selector(new TFirst[0], _secondEnumerator.Current.Value));

            while (_secondEnumerator.MoveNext())
                yield return new Interval<TResult>(_secondEnumerator.Current, _selector(new TFirst[0], _secondEnumerator.Current.Value));
        }

        private bool TryPick<T>(IEnumerator<Interval<T[]>> enumerator, out Interval<T[]> elem)
        {
            if (enumerator.MoveNext())
            {
                elem = enumerator.Current;
                return true;
            }
            else
            {
                elem = null;
                return false;
            }
        }
    }
}
