// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal static class EnumerableExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new(source);
    }

    public static string ToString<T>(this IEnumerable<T> collection, char separator)
    {
        return string.Join(separator, collection);
    }

    public static IEnumerable<IGrouping<TKey, TKey>> GroupKeys<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> source, Func<TValue, bool> predicate)
        where TKey : notnull
    {
        return source.SelectMany(dict => dict.Where(kv => predicate(kv.Value)).Select(kv => kv.Key)).GroupBy(k => k);
    }
}