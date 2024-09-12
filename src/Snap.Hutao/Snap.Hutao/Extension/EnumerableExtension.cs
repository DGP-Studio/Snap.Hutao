// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Snap.Hutao.Extension;

internal static partial class EnumerableExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSource? FirstOrFirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        TSource? first = default;

        foreach (TSource element in source)
        {
            first ??= element;

            if (predicate(element))
            {
                return element;
            }
        }

        return first;
    }

    public static string JoinToString<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, char separator, Action<StringBuilder, TKey, TValue> selector)
    {
        StringBuilder resultBuilder = new();

        IEnumerator<KeyValuePair<TKey, TValue>> enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return string.Empty;
        }

        KeyValuePair<TKey, TValue> first = enumerator.Current;
        selector(resultBuilder, first.Key, first.Value);

        if (!enumerator.MoveNext())
        {
            return resultBuilder.ToString();
        }

        do
        {
            resultBuilder.Append(separator);
            KeyValuePair<TKey, TValue> current = enumerator.Current;
            selector(resultBuilder, current.Key, current.Value);
        }
        while (enumerator.MoveNext());

        return resultBuilder.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }

    public static string ToString<T>(this IEnumerable<T> collection, char separator)
    {
        return string.Join(separator, collection);
    }
}