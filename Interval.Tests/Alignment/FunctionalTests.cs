using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Intervals.Alignments.Tests
{
    public class FunctionalTests
    {
        [Fact]
        public void Test()
        {
            var a1 = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new [] { 1 }, new char[0]),
                new AlignedPair<int, char>(new [] { 2 }, new [] { 'C', 'D'}),
                new AlignedPair<int, char>(new int[0], new [] { 'E' }),
                new AlignedPair<int, char>(new [] { 1 }, new char[0]),
                new AlignedPair<int, char>(new int[0], new [] { 'F' }),
                new AlignedPair<int, char>(new [] { 3 }, new char[0]),
            });

            Assert.Equal(a1, a1.DetachLeft().AttachLeft(a1.GetRight()));
            Assert.Equal(a1, a1.DetachRight().AttachRight(a1.GetLeft()));

            var a2 = Alignment.Create(new[]
            {
                new AlignedPair<int, char>(new [] { 4 }, new char[0]),
                new AlignedPair<int, char>(new int[0], new [] { 'G', 'H' }),
                new AlignedPair<int, char>(new [] { 5, 6 }, new [] { 'I' }),
            });

            var a3 = a1.Concat(a2);

            Assert.Equal(a3, a1.DetachRight().Join(a2.DetachRight().Translate(a1.GetLeft().Length)).AttachRight(a3.GetLeft()));
            Assert.Equal(a3, a1.DetachLeft().Join(a2.DetachLeft().Translate(a1.GetRight().Length)).AttachLeft(a3.GetRight()));
        }
    }
}
