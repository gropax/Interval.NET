using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interval
{
    public static class StringExtensions
    {
        public static Interval<string> ToInterval(this string s)
        {
            return new Interval<string>(0, s.Length, s);
        }

        public static string Replace<T>(this string s, SortedIntervals<T> intervals, Func<T, string> replacement)
        {
            return s.Replace(intervals, i => replacement(i.Value));
        }

        public static string Replace<T>(this string s, SortedIntervals<T> intervals, Func<Interval<T>, string> replacement)
        {
            var sb = new StringBuilder();

            try
            {
                var idx = 0;
                Interval<T> lastInterval = null;
                foreach (var interval in intervals)
                {
                    if (lastInterval != null && interval.Start < lastInterval.End)
                        throw new ArgumentException("Intervals should not overlap.");

                    sb.Append(s.Substring(idx, interval.Start - idx));

                    string replace = replacement(interval) ?? s.Substring(interval);
                    sb.Append(replace);

                    idx = interval.End;
                    lastInterval = interval;
                }
                sb.Append(s.Substring(idx, s.Length - idx));
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("Interval exceeding string dimensions.", e);
            }

            return sb.ToString();
        }

        public static Alignment<T> AlignReplace<T>(this string s, SortedIntervals<T> intervals, Func<string, string> replacement)
        {
            var sb = new StringBuilder();
            string toString;
            var intervalAlignments = new List<IntervalAlignment<T>>();
            var mappings = new Dictionary<int, int>();

            try
            {
                var fromIdx = 0;
                var toIdx = 0;
                mappings.Add(0, 0);

                Interval<T> lastInterval = null;
                foreach (var fromInterval in intervals)
                {
                    if (lastInterval != null && fromInterval.Start < lastInterval.End)
                        throw new ArgumentException("Intervals should not overlap.");

                    // Append string segment before interval
                    string before = s.Substring(fromIdx, fromInterval.Start - fromIdx);
                    sb.Append(before);
                    toIdx += before.Length;

                    var fromContent = s.Substring(fromInterval);

                    // Append string replacement
                    var toContent = replacement(fromContent);
                    sb.Append(toContent);

                    // Store from and to intervals
                    var toInterval = new Interval<T>(start: toIdx, length: toContent.Length, value: fromInterval.Value);
                    intervalAlignments.Add(new IntervalAlignment<T>(fromInterval, toInterval));

                    // Store mappings for intervals start and end positions
                    mappings[fromInterval.Start] = toInterval.Start;
                    mappings[fromInterval.End] = toInterval.End;

                    fromIdx = fromInterval.End;
                    toIdx = toInterval.End;

                    lastInterval = fromInterval;
                }

                // Append string segment after last interval
                sb.Append(s.Substring(fromIdx, s.Length - fromIdx));

                // Finalize TO string and store mapping for end of strings
                toString = sb.ToString();
                mappings[s.Length] = toString.Length;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("Interval exceeding string dimensions.", e);
            }

            return new Alignment<T>(
                left: s,
                right: sb.ToString(),
                intervals: intervalAlignments.ToArray(),
                mappings: mappings);
        }

        public static string Substring(this string s, IInterval i)
        {
            try
            {
                return s.Substring(i.Start(), i.Length());
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("Interval exceeding string dimensions.", e);
            }
        }


        //public static IEnumerable<string> Slices<T>(this string s, IEnumerable<IInterval<T>> intervals)
        //{
        //    IInterval<T> lastInterval = null;
        //    foreach (var interval in intervals.Sorted())
        //    {
        //        if (lastInterval != null && interval.Start < lastInterval.End)
        //            throw new ArgumentException("Intervals should not overlap.");

        //        string slice;
        //        try
        //        {
        //            slice = s.Substring(interval.Start, interval.Length);
        //        }
        //        catch (ArgumentOutOfRangeException e)
        //        {
        //            throw new ArgumentOutOfRangeException("Interval exceeding string dimensions.");
        //        }

        //        lastInterval = interval;
        //        yield return slice;
        //    }
        //}

        public static string Replace(this string s, Regex regex, Func<Match, string> matched = null,
            Func<string, string> unmatched = null)
        {
            var builder = new StringBuilder();
            int i = 0;

            foreach (Match match in regex.Matches(s))
            {
                string before = s.Substring(i, match.Index - i);
                if (unmatched != null)
                    before = unmatched(before);

                string covered = match.Value;
                if (matched != null)
                    covered = matched(match);

                builder.Append(before);
                builder.Append(covered);

                i = match.Index + match.Length;
            }

            string after = s.Substring(i);
            if (unmatched != null)
                after = unmatched(after);

            builder.Append(after);

            return builder.ToString();
        }

        public static IEnumerable<Interval<string>> IntervalSplit(this string s, string separator,
            StringSplitOptions options = StringSplitOptions.None)
        {
            int pos = 0;
            foreach (var segment in s.Split(new[] { separator }, StringSplitOptions.None))
            {
                if (segment.Length != 0 || options == StringSplitOptions.None)
                {
                    yield return new Interval<string>(pos, segment.Length, segment);
                    pos += segment.Length;
                }
                pos += separator.Length;
            }
        }
    }
}
