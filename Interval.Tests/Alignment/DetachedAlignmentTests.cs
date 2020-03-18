using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Intervals.Alignments.Tests
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
        public void TestConstructor_ConsecutiveZeroLengthValue()
        {
            var alignment = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(0, 3, new [] { 'A', 'B' }),
                new Interval<char[]>(3, 2, new char[0]),
                new Interval<char[]>(5, 3, new char[0]),
                new Interval<char[]>(8, 1, new [] { 'C', 'D' }),
            });

            var expected = new[]
            {
                new Interval<char[]>(0, 3, new [] { 'A', 'B' }),
                new Interval<char[]>(3, 5, new char[0]),
                new Interval<char[]>(8, 1, new [] { 'C', 'D' }),
            };

            Assert.Equal(expected, alignment.Intervals);
        }

        [Fact]
        public void TestConstructor_ConsecutiveZeroLengthInterval()
        {
            var alignment = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(0, 5, new [] { 'A' }),
                new Interval<char[]>(5, 0, new [] { 'B' }),
                new Interval<char[]>(5, 0, new [] { 'C' }),
                new Interval<char[]>(5, 3, new [] { 'D' }),
            });

            var expected = new[]
            {
                new Interval<char[]>(0, 5, new [] { 'A' }),
                new Interval<char[]>(5, 0, new [] { 'B', 'C' }),
                new Interval<char[]>(5, 3, new [] { 'D' }),
            };

            Assert.Equal(expected, alignment.Intervals);
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


        [Fact]
        public void TestValueAndToInterval()
        {
            var alignment = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(-2, 0, new [] { 'X' }),
                new Interval<char[]>(-2, 3, new [] { 'A', 'B' }),
                new Interval<char[]>(3, 0, new [] { 'C', }),
                new Interval<char[]>(3, 5, new char[0]),
                new Interval<char[]>(8, 2, new [] { 'D' }),
                new Interval<char[]>(12, 0, new [] { 'E' }),
                new Interval<char[]>(12, 3, new [] { 'F', 'G' }),
                new Interval<char[]>(15, 0, new [] { 'H' }),
            });

            var expectedValue = new char[] { 'X', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            Assert.Equal(expectedValue, alignment.GetValue());

            var expectedInterval = new Interval<char[]>(-2, 17, expectedValue);
            Assert.Equal(expectedInterval, alignment.ToInterval());
        }


        [Fact]
        public void TestJoin_EndsMeet()
        {
            var left = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(0, 5, new [] { 'A' }),
                new Interval<char[]>(7, 5, new [] { 'B' }),
                new Interval<char[]>(12, 2, new [] { 'C' }),
            });
            var right = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(14, 5, new [] { 'D' }),
                new Interval<char[]>(19, 2, new [] { 'E' }),
                new Interval<char[]>(21, 4, new [] { 'F' }),
            });

            var alignment = left.Join(right);
            var expected = left.Intervals.Concat(right.Intervals).ToArray();

            Assert.Equal(expected, alignment.Intervals);
        }

        [Fact]
        public void TestJoin_ConsecutiveZeroSpace()
        {
            var left = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(0, 7, new [] { 'A' }),
                new Interval<char[]>(7, 5, new [] { 'B' }),
                new Interval<char[]>(12, 0, new [] { 'C' }),
            });
            var right = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(12, 0, new [] { 'D' }),
                new Interval<char[]>(12, 2, new [] { 'E' }),
                new Interval<char[]>(14, 4, new [] { 'F' }),
            });

            var expected = new DetachedAlignment<char>(new[]
            {
                new Interval<char[]>(0, 7, new [] { 'A' }),
                new Interval<char[]>(7, 5, new [] { 'B' }),
                new Interval<char[]>(12, 0, new [] { 'C', 'D' }),
                new Interval<char[]>(12, 2, new [] { 'E' }),
                new Interval<char[]>(14, 4, new [] { 'F' }),
            });

            var alignment = left.Join(right);
            Assert.Equal(expected, alignment);
        }

        [Fact]
        public void TestJoin_GapBetweenAlignments()
        {
            var left = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(0, 5, new [] { 'A' }),
                new Interval<char[]>(7, 5, new [] { 'B' }),
                new Interval<char[]>(12, 2, new [] { 'C' }),
            });
            var right = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(15, 0, new [] { 'D' }),
                new Interval<char[]>(15, 2, new [] { 'E' }),
                new Interval<char[]>(17, 4, new [] { 'F' }),
            });

            var alignment = left.Join(right);
            var expected = left.Intervals
                .Concat(new[] { new Interval<char[]>(14, 1, new char[0]) })
                .Concat(right.Intervals).ToArray();

            Assert.Equal(expected, alignment.Intervals);
        }

        [Fact]
        public void TestJoin_OverlappingIntervals()
        {
            var left = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(0, 5, new [] { 'A' }),
                new Interval<char[]>(7, 5, new [] { 'B' }),
                new Interval<char[]>(12, 4, new [] { 'C' }),
            });
            var right = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(15, 0, new [] { 'D' }),
                new Interval<char[]>(15, 2, new [] { 'E' }),
                new Interval<char[]>(17, 4, new [] { 'F' }),
            });

            Assert.Throws<DetachedAlignmentException>(() => left.Join(right));
        }



        [Fact]
        public void TestAttach_CoverAllSequence()
        {
            var expected = new Alignment<char, int>(new[]
            {
                new AlignedPair<char, int>(new [] { 'A'}, new int[0]),
                new AlignedPair<char, int>(new [] { 'B', 'C' }, new [] { 1, 2, 3 }),
                new AlignedPair<char, int>(new [] { 'D'}, new [] { 4, 5, 6 }),
                new AlignedPair<char, int>(new [] { 'E', 'F', 'G' }, new [] { 7 }),
                new AlignedPair<char, int>(new char[0], new [] { 8, 9 }),
                new AlignedPair<char, int>(new [] { 'H' }, new [] { 10 }),
            });

            var leftAlignment = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(0, 0, new [] { 'A' }),
                new Interval<char[]>(0, 3, new [] { 'B', 'C' }),
                new Interval<char[]>(3, 3, new [] { 'D' }),
                new Interval<char[]>(6, 1, new [] { 'E', 'F', 'G' }),
                new Interval<char[]>(7, 2, new char[0]),
                new Interval<char[]>(9, 1, new [] { 'H' }),
            });
            var right = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Assert.Equal(expected, leftAlignment.AttachLeft(right));

            var rightAlignment = DetachedAlignment.Create(new[]
            {
                new Interval<int[]>(0, 1, new int[0]),
                new Interval<int[]>(1, 2, new [] { 1, 2, 3 }),
                new Interval<int[]>(3, 1, new [] { 4, 5, 6 }),
                new Interval<int[]>(4, 3, new [] { 7 }),
                new Interval<int[]>(7, 0, new [] { 8, 9 }),
                new Interval<int[]>(7, 1, new [] { 10 }),
            });
            var left = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            Assert.Equal(expected, rightAlignment.AttachRight(left));
        }

        [Fact]
        public void TestAttach_PartOfSequence()
        {
            var expected = new Alignment<char, int>(new[]
            {
                new AlignedPair<char, int>(new [] { 'D'}, new [] { 4, 5, 6 }),
                new AlignedPair<char, int>(new [] { 'E', 'F', 'G' }, new [] { 7 }),
                new AlignedPair<char, int>(new char[0], new [] { 8, 9 }),
            });

            var leftAlignment = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(3, 3, new [] { 'D' }),
                new Interval<char[]>(6, 1, new [] { 'E', 'F', 'G' }),
                new Interval<char[]>(7, 2, new char[0]),
            });
            var right = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Assert.Equal(expected, leftAlignment.AttachLeft(right));

            var rightAlignment = DetachedAlignment.Create(new[]
            {
                new Interval<int[]>(3, 1, new [] { 4, 5, 6 }),
                new Interval<int[]>(4, 3, new [] { 7 }),
                new Interval<int[]>(7, 0, new [] { 8, 9 }),
            });
            var left = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            Assert.Equal(expected, rightAlignment.AttachRight(left));
        }

        [Fact]
        public void TestAttach_IndexOutOfBounds()
        {
            var seq = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var alignment = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(-2, 3, new [] { 'D' }),
                new Interval<char[]>(1, 2, new [] { 'E', 'F', 'G' }),
                new Interval<char[]>(3, 5, new char[0]),
            });

            Assert.Throws<DetachedAlignmentException>(() => alignment.AttachLeft(seq));
            Assert.Throws<DetachedAlignmentException>(() => alignment.AttachRight(seq));

            alignment = DetachedAlignment.Create(new[]
            {
                new Interval<char[]>(1, 3, new [] { 'D' }),
                new Interval<char[]>(4, 2, new [] { 'E', 'F', 'G' }),
                new Interval<char[]>(6, 5, new char[0]),
            });

            Assert.Throws<DetachedAlignmentException>(() => alignment.AttachLeft(seq));
            Assert.Throws<DetachedAlignmentException>(() => alignment.AttachRight(seq));
        }
    }
}
