// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="List{T}"/> 部分
/// </summary>
public static partial class EnumerableExtension
{
    /// <inheritdoc cref="Enumerable.Average(IEnumerable{int})"/>
    public static double AverageNoThrow(this List<int> source)
    {
        Span<int> span = CollectionsMarshal.AsSpan(source);

        if (span.IsEmpty)
        {
            return 0;
        }

        long sum = 0;

        for (int i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }

        return (double)sum / span.Length;
    }

    /// <summary>
    /// 如果传入列表不为空则原路返回，
    /// 如果传入列表为空返回一个空的列表
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>源列表或空列表</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<TSource> EmptyIfNull<TSource>(this List<TSource>? source)
    {
        return source ?? new();
    }

    /// <summary>
    /// 移除表中首个满足条件的项
    /// </summary>
    /// <typeparam name="T">项的类型</typeparam>
    /// <param name="list">表</param>
    /// <param name="shouldRemovePredicate">是否应当移除</param>
    /// <returns>是否移除了元素</returns>
    public static bool RemoveFirstWhere<T>(this IList<T> list, Func<T, bool> shouldRemovePredicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (shouldRemovePredicate.Invoke(list[i]))
            {
                list.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
