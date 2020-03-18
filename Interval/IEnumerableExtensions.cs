using System;
using System.Collections.Generic;
using System.Text;

namespace Interval
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<U> SqueezeReduce<T, U, X>(this IEnumerable<T> ts,
            Func<T, X> selector, Func<T[], U> reducer)
        {
            X lastValue = default(X);
            var group = new List<T>();

            foreach (var t in ts)
            {
                X value = selector(t);
                bool isDefault = value.Equals(default(X));
                bool matchLast = value.Equals(lastValue);

                if (!isDefault && matchLast)
                    group.Add(t);
                else
                {
                    if (group.Count > 0)
                    {
                        yield return reducer(group.ToArray());
                        group.Clear();
                    }

                    if (isDefault)
                        yield return reducer(new[] { t });
                    else
                        group.Add(t);
                }

                lastValue = value;
            }

            if (group.Count > 0)
                yield return reducer(group.ToArray());
        }
    }
}
