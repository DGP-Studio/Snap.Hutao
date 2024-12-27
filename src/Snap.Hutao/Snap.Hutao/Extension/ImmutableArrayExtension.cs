// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Extension;

internal static class ImmutableArrayExtension
{
    [Pure]
    public static ImmutableArray<TElement> EmptyIfDefault<TElement>(this ImmutableArray<TElement> array)
    {
        return array.IsDefault ? [] : array;
    }

    [Pure]
    public static ImmutableArray<TElement> Reverse<TElement>(this ImmutableArray<TElement> array)
    {
        if (array.IsEmpty)
        {
            return array;
        }

        TElement[] reversed = GC.AllocateUninitializedArray<TElement>(array.Length);
        array.AsSpan().CopyTo(reversed);
        Array.Reverse(reversed);
        return ImmutableCollectionsMarshal.AsImmutableArray(reversed);
    }

    [Pure]
    public static ImmutableArray<TResult> SelectAsArray<TSource, TResult>(this ImmutableArray<TSource> array, Func<TSource, TResult> selector)
    {
        return ImmutableArray.CreateRange(array, selector);
    }

    [Pure]
    public static ImmutableArray<TResult> SelectAsArray<TSource, TResult>(this ImmutableArray<TSource> array, Func<TSource, int, TResult> selector)
    {
        int length = array.Length;
        if (length == 0)
        {
            return [];
        }

        ReadOnlySpan<TSource> sourceSpan = array.AsSpan();
        TResult[] results = GC.AllocateUninitializedArray<TResult>(length);

        Span<TResult> resultSpan = results.AsSpan();
        for (int index = 0; index < sourceSpan.Length; index++)
        {
            resultSpan[index] = selector(sourceSpan[index], index);
        }

        return ImmutableCollectionsMarshal.AsImmutableArray(results);
    }
}