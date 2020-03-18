using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intervals
{
    public interface IInterval
    {
        Interval ToInterval();
    }

    public interface IInterval<T> : IInterval
    {
        new Interval<T> ToInterval();
    }
}
