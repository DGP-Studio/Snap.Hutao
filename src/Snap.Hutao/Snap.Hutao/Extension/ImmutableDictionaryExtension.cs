// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Snap.Hutao.Extension;

internal static class ImmutableDictionaryExtension
{
    [Pure]
    public static ImmutableDictionary<TKey, TSource> ToImmutableDictionaryIgnoringDuplicateKeys<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        where TKey : notnull
    {
        ImmutableDictionary<TKey, TSource>.Builder builder = ImmutableDictionary.CreateBuilder<TKey, TSource>();

        foreach (TSource value in source)
        {
            builder[keySelector(value)] = value;
        }

        return builder.ToImmutable();
    }

    [Pure]
    public static ImmutableDictionary<TKey, TValue> ToImmutableDictionaryIgnoringDuplicateKeys<TKey, TValue, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        where TKey : notnull
    {
        ImmutableDictionary<TKey, TValue>.Builder builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();

        foreach (TSource value in source)
        {
            builder[keySelector(value)] = valueSelector(value);
        }

        return builder.ToImmutable();
    }
}