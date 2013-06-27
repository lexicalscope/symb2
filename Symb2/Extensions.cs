using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symb2
{
    public delegate IEnumerable<T> Transform<T>(IEnumerable<T> ts);
    public delegate Tuple<IEnumerable<T>, IEnumerable<T>> PairTransform<T>(IEnumerable<T> ts1, IEnumerable<T> ts2);
 
    public delegate void Matched<T>(IEnumerable<T> scanned, IEnumerable<T> ts);
    public delegate void UnMatched<T>(IEnumerable<T> scanned);

    public static class Extensions
    {
        public static Tuple<IList<T>, IList<T>> ReplaceSubsequences<T>(
            IEnumerable<T> sequence1,
            IEnumerable<T> sequence2,
            PairTransform<T> transform,
            IEnumerable<Predicate<T>> predicates1,
            IEnumerable<Predicate<T>> predicates2)
        {
            var result1 = new List<T>(); 
            var result2 = new List<T>();
 
            IEnumerable<T> current1 = sequence1;
            IEnumerable<T> current2 = sequence2;
            while (predicates1.Any() && current1.Any() && predicates2.Any() && current2.Any())
            {
                current1 = current1.TakeMatch(
                        (scanned1, ts1) => current2 = current2.TakeMatch(
                            (scanned2, ts2) => { 
                                var transformed = transform(ts1, ts2);
                                result1.AddRange(scanned1);
                                result1.AddRange(transformed.Item1);
                                result2.AddRange(scanned2);
                                result2.AddRange(transformed.Item2);},
                            scanned2 => { 
                                result1.AddRange(scanned1); 
                                result1.AddRange(ts1); 
                                result2.AddRange(scanned2);},
                            predicates2),
                        scanned1 => result1.AddRange(scanned1),
                        predicates1);
            }
            result1.AddRange(current1);
            result2.AddRange(current2);
            return new Tuple<IList<T>,IList<T>>(result1, result2);
        }

        public static IEnumerable<T> TakeMatch<T>(
            this IEnumerable<T> sequence,
            Matched<T> onMatch,
            UnMatched<T> onNoMatch,
            IEnumerable<Predicate<T>> predicates)
        {
            IEnumerable<T> pending = sequence;
            List<T> scanned = new List<T>();

            while (pending.Any() && predicates.Any())
            {
                var window = pending.
                    TakeWhile((elem, index) =>
                        index < predicates.Count() &&
                        predicates.ElementAt(index)(elem));
                if (window.Count() == predicates.Count())
                {
                    onMatch(scanned, window);
                    return pending.Skip(predicates.Count());
                }
                else
                {
                    scanned.Add(pending.First());
                    pending = pending.Skip(1);
                }
            }
            onNoMatch(scanned);
            return pending;
        }

        public static IList<T> ReplaceSubsequences<T>(
            this IEnumerable<T> sequence,
            Transform<T> transform,
            params Predicate<T>[] predicates)
        {
            var result = new List<T>(); 
            IEnumerable<T> current = sequence;
            while (predicates.Length > 0 && current.Count() >= predicates.Length)
            {
                var window = current.
                    TakeWhile((elem, index) =>
                        index < predicates.Length &&
                        predicates[index](elem));
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
            result.AddRange(current);
            return result;
        }
    }
}
