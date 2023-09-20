// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="Collection{T}"/> 部分
/// </summary>
internal static partial class EnumerableExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<TSource>([NotNullWhen(false)][MaybeNullWhen(true)] this Collection<TSource>? source)
    {
        if (source is { Count: > 0 })
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 移除集合中满足条件的项
    /// </summary>
    /// <typeparam name="T">集合项类型</typeparam>
    /// <param name="collection">集合</param>
    /// <param name="shouldRemovePredicate">是否应当移除</param>
    /// <returns>移除的个数</returns>
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
