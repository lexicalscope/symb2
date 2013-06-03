using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symb2
{
    public delegate IEnumerable<T> Transform<T>(IEnumerable<T> ts);

    public static class Extensions
    {
        public static IList<T> ReplaceSubsequences<T>(
            this IEnumerable<T> sequence,
            Transform<T> transform,
            params Predicate<T>[] predicates)
        {
            Console.WriteLine("input:" + string.Join(",", sequence));

            var result = new List<T>();
            IEnumerable<T> current = sequence;
            while (current.Count() >= predicates.Length)
            {
                var window = current.
                    TakeWhile((elem, index) =>
                        index < predicates.Length &&
                        predicates[index](elem));
                Console.WriteLine("loop:" + string.Join(",", window));
                if (window.Count() == predicates.Length)
                {
                    result.AddRange(transform(window));
                    current = current.Skip(predicates.Length);
                }
                else
                {
                    result.Add(current.First());
                    current = current.Skip(1);
                }
            }
            return result;
        }
    }
}
