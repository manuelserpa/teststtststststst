using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.amsOSRAM.Common.Extensions
{
    /// <summary>
    /// Extensions to extend Linq functionalities
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Method to order and group IEnumerables
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOrderKey"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="orderBy"></param>
        /// <param name="keySelector"></param>
        /// <param name="join"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<TSource>> JoinBy<TSource, TOrderKey, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TOrderKey> orderBy,
            Func<TSource, TKey> keySelector,
            Func<TKey, TKey, bool> join)
        {
            List<List<TSource>> results = new List<List<TSource>>();
            TSource[] orderedSource = new List<TSource>(source).OrderBy(orderBy).ToArray();

            if (orderedSource.Length > 0)
            {
                List<TSource> group = new List<TSource> { orderedSource[0] };
                results.Add(group);
                if (orderedSource.Length > 1)
                {
                    for (int i = 1; i < orderedSource.Length; i++)
                    {
                        TSource lag = orderedSource[i - 1];
                        TSource current = orderedSource[i];
                        if (join(keySelector(lag), keySelector(current)))
                        {
                            group.Add(current);
                        }
                        else
                        {
                            group = new List<TSource> { current };
                            results.Add(group);
                        }
                    }
                }
            }

            return results;
        }
    }
}
