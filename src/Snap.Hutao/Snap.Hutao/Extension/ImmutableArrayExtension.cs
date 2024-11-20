// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Snap.Hutao.Extension;

internal static class ImmutableArrayExtension
{
    public static ImmutableArray<TElement> EmptyIfDefault<TElement>(this ImmutableArray<TElement> array)
    {
        return array.IsDefault ? [] : array;
    }

    [Pure]
    public static ImmutableArray<TResult> SelectAsArray<TSource, TResult>(this ImmutableArray<TSource> array, Func<TSource, TResult> selector)
    {
        return ImmutableArray.CreateRange(array, selector);
    }

    [Pure]
    public static ImmutableArray<TResult> SelectAsArray<TSource, TResult>(this ImmutableArray<TSource> array, Func<TSource, int, TResult> selector)
    {
        ReadOnlySpan<TSource> span = array.AsSpan();
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>(span.Length);

        int index = -1;
        foreach (ref readonly TSource item in span)
        {
            builder.Add(selector(item, ++index));
        }

        return builder.ToImmutable();
    }
}