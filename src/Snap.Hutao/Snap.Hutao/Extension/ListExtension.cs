// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Extension;

internal static class ListExtension
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static double Average(this List<int> source)
    {
        if (source.Count <= 0)
        {
            return 0;
        }

        long sum = 0;
        foreach (int item in source)
        {
            sum += item;
        }

        return (double)sum / source.Count;
    }

    [Pure]
    public static T? BinarySearch<TItem, T>(this List<T> list, TItem item, Func<TItem, T, int> compare)
        where T : class
    {
#if NET10_0_OR_GREATER
        return CollectionsMarshal.AsSpan(list).BinarySearch(item, compare);
#error Remove this when .NET 10.0 is available
#else
        return SpanExtension.BinarySearch(CollectionsMarshal.AsSpan(list), item, compare);
#endif
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