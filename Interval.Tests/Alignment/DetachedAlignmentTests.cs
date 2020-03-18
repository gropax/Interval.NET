using System;
using System.Linq;
using Xunit;

namespace Interval.Alignment.Tests
{
    public class DetachedAlignmentTests
    {
        [Fact]
        public void TestConstructor_StringConversion()
        {
            var intervals = new[]
            {
                new Interval<string>(0, 3, "ABC"),
                new Interval<string>(3, 5, "DE"),
                new Interval<string>(8, 3, "GH"),
            };

            var alignment = DetachedAlignment.Create(intervals);
            var expected = intervals.Select(i => i.Map(s => s.ToCharArray())).ToArray();

            Assert.Equal(expected, alignment.Intervals);
        }

        [Fact]
        public void TestConstructor_ZeroLengthInside()
        {
            var intervals = new[]
            {
                new Interval<char[]>(0, 3, new [] { 'A' }),
                new Interval<char[]>(3, 5, new [] { 'B' }),
                new Interval<char[]>(8, 0, new [] { 'C' }),
                new Interval<char[]>(8, 3, new [] { 'D' }),
            };

            var alignment = DetachedAlignment.Create(intervals);

            Assert.Equal(intervals, alignment.Intervals);
        }

        [Fact]
        public void TestConstructor_ZeroLengthFirst()
        {
            var intervals = new[]
            {
                new Interval<char[]>(0, 0, new [] { 'A' }),
                new Interval<char[]>(0, 5, new [] { 'B' }),
                new Interval<char[]>(5, 3, new [] { 'C' }),
                new Interval<char[]>(8, 3, new [] { 'D' }),
            };

            var alignment = DetachedAlignment.Create(intervals);

            Assert.Equal(intervals, alignment.Intervals);
        }

        [Fact]
        public void TestConstructor_ZeroLengthLast()
        {
            var intervals = new[]
            {
                new Interval<char[]>(0, 3, new [] { 'A' }),
                new Interval<char[]>(3, 2, new [] { 'B' }),
                new Interval<char[]>(5, 5, new [] { 'C' }),
                new Interval<char[]>(10, 0, new [] { 'D' }),
            };

            var alignment = DetachedAlignment.Create(intervals);

            Assert.Equal(intervals, alignment.Intervals);
        }

        [Fact]
        public void TestConstructor_ConsecutiveZeroLength()
        {
            var intervals = new[]
            {
                new Interval<char[]>(0, 5, new [] { 'A' }),
                new Interval<char[]>(5, 0, new [] { 'B' }),
                new Interval<char[]>(5, 0, new [] { 'C' }),
                new Interval<char[]>(5, 3, new [] { 'D' }),
            };

            Assert.Throws<DetachedAlignmentException>(() =>
                DetachedAlignment.Create(intervals));
        }

        [Fact]
        public void TestConstructor_OverlappingIntervals()
        {
            var intervals = new[]
            {
                new Interval<char[]>(0, 5, new [] { 'A' }),
                new Interval<char[]>(5, 5, new [] { 'B' }),
                new Interval<char[]>(8, 5, new [] { 'C' }),
            };

            Assert.Throws<DetachedAlignmentException>(() =>
                DetachedAlignment.Create(intervals));
        }

        [Fact]
        public void TestConstructor_GapBetweenIntervals()
        {
            var intervals = new[]
            {
                new Interval<char[]>(0, 5, new [] { 'A' }),
                new Interval<char[]>(7, 5, new [] { 'B' }),
                new Interval<char[]>(12, 2, new [] { 'C' }),
            };

            var expected = new[]
            {
                new Interval<char[]>(0, 5, new [] { 'A' }),
                new Interval<char[]>(5, 2, new char[0]),
                new Interval<char[]>(7, 5, new [] { 'B' }),
                new Interval<char[]>(12, 2, new [] { 'C' }),
            };

            var alignment = DetachedAlignment.Create(intervals);

            Assert.Equal(expected, alignment.Intervals);
        }
    }
}
