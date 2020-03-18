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
            var alignment = new Alignment<int, char>(new[]
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
        public void TestReverse()
        {
            var alignment = new Alignment<int, char>(new[]
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

            var reversed = alignment.Reverse();

            Assert.Equal(expected, reversed);
            Assert.Equal(alignment, reversed.Reverse());
        }
    }
}
