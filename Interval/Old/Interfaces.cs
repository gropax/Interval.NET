using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interval
{
    public interface IGraph<T>
    {
        T[] Vertices { get; }
    }

    public interface ITree<T> : IGraph<T>
    {
    }
}
