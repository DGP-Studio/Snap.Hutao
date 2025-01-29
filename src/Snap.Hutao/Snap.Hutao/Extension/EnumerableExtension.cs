// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal static class EnumerableExtension
{
    public static IEnumerable<KeyValuePair<TKey, int>> CountByKey<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> source, Func<TValue, bool> predicate)
        where TKey : notnull
    {
        return source.SelectMany(map => map.Where(kv => predicate(kv.Value))).CountBy(kv => kv.Key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new(source);
    }

    public static string ToString<T>(this IEnumerable<T> collection, char separator)
    {
        return string.Join(separator, collection);
    }
}