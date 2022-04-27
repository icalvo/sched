using System.Diagnostics;

namespace Scheduler;

public static class EnumerableExtensions
{
    public static string StringJoin<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
    }
    
    public static IEnumerable<T> From<T>(params T[] items)
    {
        return items.AsEnumerable();
    }

    /// <summary>
    /// Weaves several sorted sequences into a single one (also sorted), based on a sorting function.
    /// </summary>
    /// <param name="lists"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// The provided sequences are supposed to be already sorted. This function uses that fact to provide a
    /// final sorted sequence with all the elements of the provided ones, without having to completely enumerate
    /// one of the sequences before the others. Therefore, this algorithm allows the sequences to be indefinitely long.
    ///
    /// If any of the sequences are unsorted, the unsorted elements (those that are smaller than the predecessor)
    /// will be dropped out of the resulting sequence.
    /// </returns>
    public static IEnumerable<T> Weave<T>(this IEnumerable<IEnumerable<T>> lists) where T : IComparable<T>
    {
        return lists.Weave(x => x);
    }

    public static IEnumerable<T> Weave<T>(params IEnumerable<T>[] lists) where T : IComparable<T>
    {
        return Weave(lists.AsEnumerable());
    }
    
    public static IEnumerable<T> Weave<T, TKey>(Func<T, TKey> selector, params IEnumerable<T>[] lists) where TKey : IComparable<TKey>
    {
        return Weave(lists.AsEnumerable(), selector);
    }
    
    public static IEnumerable<T> Weave<T, TKey>(this IEnumerable<IEnumerable<T>> lists, Func<T, TKey> selector) where TKey : IComparable<TKey>
    {
        var enumerators = lists.Select(x => x.GetEnumerator()).ToList();
        try
        {
            enumerators.RemoveAll(x => !x.MoveNext());

            while (enumerators.Any())
            {
                var minEnumerator = enumerators.MinBy(x => selector(x.Current))!;

                yield return minEnumerator.Current;
                Debug.Assert(minEnumerator != null, nameof(minEnumerator) + " != null");
                if (!minEnumerator.MoveNext())
                {
                    enumerators.Remove(minEnumerator);
                }
            }
        }
        finally
        {
            foreach (var enumerator in enumerators)
            {
                enumerator.Dispose();
            }
        }
    }
}