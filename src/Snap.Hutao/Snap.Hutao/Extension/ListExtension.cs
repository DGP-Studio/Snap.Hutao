// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Extension;

internal static class ListExtension
{
    [Pure]
    public static double Average(this List<int> source)
    {
        Span<int> span = CollectionsMarshal.AsSpan(source);
        if (span.IsEmpty)
        {
            return 0;
        }

        long sum = 0;
        foreach (ref readonly int item in span)
        {
            sum += item;
        }

        return (double)sum / span.Length;
    }

    [Pure]
    public static T? BinarySearch<TItem, T>(this List<T> list, TItem item, Func<TItem, T, int> compare)
        where T : class
    {
        return CollectionsMarshal.AsSpan(list).BinarySearch(item, compare);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<TSource> EmptyIfNull<TSource>(this List<TSource>? source)
    {
        return source ?? [];
    }

    [Pure]
    public static List<T> GetRange<T>(this List<T> list, in Range range)
    {
        (int start, int length) = range.GetOffsetAndLength(list.Count);
        return list.GetRange(start, length);
    }

    public static void RemoveLast<T>(this IList<T> collection)
    {
        collection.RemoveAt(collection.Count - 1);
    }

    [Pure]
    public static List<TResult> SelectList<TSource, TResult>(this List<TSource> list, Func<TSource, TResult> selector)
    {
        Span<TSource> span = CollectionsMarshal.AsSpan(list);
        List<TResult> results = new(span.Length);

        foreach (ref readonly TSource item in span)
        {
            results.Add(selector(item));
        }

        return results;
    }

    [Pure]
    public static List<TResult> SelectList<TSource, TResult>(this List<TSource> list, Func<TSource, int, TResult> selector)
    {
        Span<TSource> span = CollectionsMarshal.AsSpan(list);
        List<TResult> results = new(span.Length);

        int index = -1;
        foreach (ref readonly TSource item in span)
        {
            results.Add(selector(item, ++index));
        }

        return results;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static List<TSource> SortBy<TSource, TKey>(this List<TSource> list, Func<TSource, TKey> keySelector)
        where TKey : IComparable
    {
        list.Sort((left, right) => keySelector(left).CompareTo(keySelector(right)));
        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static List<TSource> SortByDescending<TSource, TKey>(this List<TSource> list, Func<TSource, TKey> keySelector)
        where TKey : IComparable
    {
        list.Sort((left, right) => keySelector(right).CompareTo(keySelector(left)));
        return list;
    }
}