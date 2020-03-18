using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Intervals.Tests
{
    public class IntervalTests
    {
        [Fact]
        public void TestImplicitConversionToRange()
        {
            var interval = new Interval(2, 3);
            var array = new int[] { 0, 1, 2, 3, 4, 5 };
            var slice = array[interval];
            Assert.Equal(new int[] { 2, 3, 4 }, slice);
        }
    }
}
