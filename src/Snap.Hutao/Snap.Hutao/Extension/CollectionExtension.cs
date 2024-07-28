// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal static class CollectionExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<TSource>(this ICollection<TSource> source)
    {
        if (source is { Count: > 0 })
        {
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<TSource>([NotNullWhen(false)][MaybeNullWhen(true)] this ICollection<TSource>? source)
    {
        if (source is { Count: > 0 })
        {
            return false;
        }

        return true;
    }

    public static int RemoveWhere<T>(this Collection<T> collection, Func<T, bool> shouldRemovePredicate)
    {
        int count = 0;
        foreach (T item in collection.ToList())
        {
            if (shouldRemovePredicate.Invoke(item))
            {
                collection.Remove(item);
                count++;
            }
        }

        return count;
    }

    public static int FirstIndexOf<T>(this Collection<T> collection, Func<T, bool> predicate)
    {
        for (int index = 0; index < collection.Count; index++)
        {
            if (predicate(collection[index]))
            {
                return index;
            }
        }

        return -1;
    }
}
