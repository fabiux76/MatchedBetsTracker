using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.BusinessLogic
{
    public static class EnumerableExtensions
    {
        static public void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T t in enumerable)
            {
                action(t);
            }
        }
    }
}