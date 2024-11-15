// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics.Contracts;

namespace Snap.Hutao.Extension;

internal static class ImmutableArrayExtension
{
    [Pure]
    public static ImmutableArray<TResult> SelectArray<TSource, TResult>(this ImmutableArray<TSource> array, Func<TSource, TResult> selector)
    {
        ReadOnlySpan<TSource> span = array.AsSpan();
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>(span.Length);

        foreach (ref readonly TSource item in span)
        {
            builder.Add(selector(item));
        }

        return builder.ToImmutable();
    }

    [Pure]
    public static unsafe ImmutableArray<TResult> SelectArray<TSource, TResult>(this ImmutableArray<TSource> array, delegate*<TSource, TResult> selector)
    {
        ReadOnlySpan<TSource> span = array.AsSpan();
        ImmutableArray<TResult>.Builder builder = ImmutableArray.CreateBuilder<TResult>(span.Length);

        foreach (ref readonly TSource item in span)
        {
            builder.Add(selector(item));
        }

        return builder.ToImmutable();
    }

    [Pure]
    public static ImmutableArray<TResult> SelectArray<TSource, TResult>(this ImmutableArray<TSource> array, Func<TSource, int, TResult> selector)
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

    [Pure]
    public static unsafe ImmutableArray<TResult> SelectArray<TSource, TResult>(this ImmutableArray<TSource> array, delegate*<TSource, int, TResult> selector)
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