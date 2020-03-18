using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Intervals.Alignments.Tests
{
    public class AlignmentTests
    {
        [Fact]
        public void TestConstructor_RemoveEmptyPairs()
        {
            var alignment = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new int[0], new char[0]),
                new AlignedPair<int, char>(new [] { 1 }, new char[0]),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new int[0], new [] { 'E' }),
                new AlignedPair<int, char>(new int[0], new char[0]),
                new AlignedPair<int, char>(new [] { 1 }, new char[0]),
                new AlignedPair<int, char>(new int[0], new [] { 'E' }),
                new AlignedPair<int, char>(new [] { 3 }, new char[0]),
                new AlignedPair<int, char>(new int[0], new char[0]),
            });

            var expected = new Alignment<int, char>(new[]
            {
                new AlignedPair<int, char>(new [] { 1 }, new char[0]),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new int[0], new [] { 'E' }),
                new AlignedPair<int, char>(new [] { 1 }, new char[0]),
                new AlignedPair<int, char>(new int[0], new [] { 'E' }),
                new AlignedPair<int, char>(new [] { 3 }, new char[0]),
            });

            Assert.Equal(expected, alignment);
        }

        [Fact]
        public void TestConstructor_SqueezeConsecutiveEmptyParts()
        {
            var alignment = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new [] { 0 }, new char[0]),
                new AlignedPair<int, char>(new [] { 1 }, new char[0]),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new int[0], new [] { 'E' }),
                new AlignedPair<int, char>(new int[0], new [] { 'F' }),
                new AlignedPair<int, char>(new int[0], new [] { 'G' }),
                new AlignedPair<int, char>(new [] { 3 }, new char[0]),
                new AlignedPair<int, char>(new [] { 4 }, new [] { 'H', 'I'}),
                new AlignedPair<int, char>(new [] { 5 }, new [] { 'J'}),
                new AlignedPair<int, char>(new int[0], new [] { 'K' }),
                new AlignedPair<int, char>(new int[0], new [] { 'L' }),
            });

            var expected = new Alignment<int, char>(new[]
            {
                new AlignedPair<int, char>(new [] { 0, 1 }, new char[0]),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new int[0], new [] { 'E', 'F', 'G' }),
                new AlignedPair<int, char>(new [] { 3 }, new char[0]),
                new AlignedPair<int, char>(new [] { 4 }, new [] { 'H', 'I'}),
                new AlignedPair<int, char>(new [] { 5 }, new [] { 'J'}),
                new AlignedPair<int, char>(new int[0], new [] { 'K', 'L' }),
            });

            Assert.Equal(expected, alignment);
        }

        [Fact]
        public void TestGetLeftAndGetRight()
        {
            var alignment = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new [] { 0, 1 }, new [] { 'A', 'B'}),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new [] { 3, 4 }, new [] { 'E' }),
                new AlignedPair<int, char>(new int[0], new [] { 'F' }),
            });

            var expectedLeft = new[] { 0, 1, 2, 3, 4 };
            Assert.Equal(expectedLeft, alignment.GetLeft());

            var expectedRight = new[] { 'A', 'B', 'C', 'D', 'E', 'F' };
            Assert.Equal(expectedRight, alignment.GetRight());
        }

        [Fact]
        public void TestSwap()
        {
            var alignment = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new [] { 0, 1 }, new [] { 'A', 'B'}),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new [] { 3, 4 }, new [] { 'E' }),
                new AlignedPair<int, char>(new int[0], new [] { 'F' }),
            });

            var expected = new Alignment<char, int>(new[]
            {
                new AlignedPair<char, int>(new [] { 'A', 'B'}, new [] { 0, 1 }),
                new AlignedPair<char, int>(new [] { 'C', 'D'}, new [] { 2 }),
                new AlignedPair<char, int>(new [] { 'E' }, new [] { 3, 4 }),
                new AlignedPair<char, int>(new [] { 'F' }, new int[0]),
            });

            var reversed = alignment.Swap();

            Assert.Equal(expected, reversed);
            Assert.Equal(alignment, reversed.Swap());
        }


        [Fact]
        public void TestConcat()
        {
            var left = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new [] { 0, 1 }, new [] { 'A', 'B'}),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new [] { 3, 4 }, new [] { 'E' }),
                new AlignedPair<int, char>(new int[0], new [] { 'F' }),
            });

            var right = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new int[0], new [] { 'G' }),
                new AlignedPair<int, char>(new [] { 5 }, new [] { 'H'}),
                new AlignedPair<int, char>(new [] { 6, 7 }, new [] { 'I'}),
            });

            var expected = new Alignment<int, char>(new[]
            {
                new AlignedPair<int, char>(new [] { 0, 1 }, new [] { 'A', 'B'}),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new [] { 3, 4 }, new [] { 'E' }),
                new AlignedPair<int, char>(new int[0], new [] { 'F', 'G' }),
                new AlignedPair<int, char>(new [] { 5 }, new [] { 'H'}),
                new AlignedPair<int, char>(new [] { 6, 7 }, new [] { 'I'}),
            });

            var concatenated = left.Concat(right);

            Assert.Equal(expected, concatenated);
        }


        [Fact]
        public void TestDetachLeft()
        {
            var alignment = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new [] { 0, 1 }, new [] { 'A', 'B'}),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new [] { 3, 4 }, new char[0]),
                new AlignedPair<int, char>(new [] { 5 }, new [] { 'E' }),
                new AlignedPair<int, char>(new int[0], new [] { 'F' }),
            });

            var expected = new DetachedAlignment<int>(new[]
            {
                new Interval<int[]>(0, 2, new [] { 0, 1 }),
                new Interval<int[]>(2, 2, new [] { 2 }),
                new Interval<int[]>(4, 0, new [] { 3, 4 }),
                new Interval<int[]>(4, 1, new [] { 5 }),
                new Interval<int[]>(5, 1, new int[0]),
            });

            var leftAlignment = alignment.DetachLeft();

            Assert.Equal(expected, leftAlignment);
        }

        [Fact]
        public void TestDetachRight()
        {
            var alignment = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new [] { 0, 1 }, new [] { 'A', 'B'}),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new [] { 3, 4 }, new char[0]),
                new AlignedPair<int, char>(new [] { 5 }, new [] { 'E' }),
                new AlignedPair<int, char>(new int[0], new [] { 'F' }),
            });

            var expected = new DetachedAlignment<char>(new[]
            {
                new Interval<char[]>(0, 2, new [] { 'A', 'B' }),
                new Interval<char[]>(2, 1, new [] { 'C', 'D' }),
                new Interval<char[]>(3, 2, new char[0]),
                new Interval<char[]>(5, 1, new [] { 'E' }),
                new Interval<char[]>(6, 0, new [] { 'F' }),
            });

            var rightAlignment = alignment.DetachRight();

            Assert.Equal(expected, rightAlignment);
        }
    }
}
