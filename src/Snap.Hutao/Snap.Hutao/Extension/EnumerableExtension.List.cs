// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="List{T}"/> 部分
/// </summary>
internal static partial class EnumerableExtension
{
    /// <inheritdoc cref="Enumerable.Average(IEnumerable{int})"/>
    public static double SpanAverage(this List<int> source)
    {
        Span<int> span = CollectionsMarshal.AsSpan(source);
        if (span.IsEmpty)
        {
            return 0;
        }

        long sum = 0;
        foreach (int item in span)
        {
            sum += item;
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
    public static bool RemoveFirstWhere<T>(this List<T> list, Func<T, bool> shouldRemovePredicate)
    {
        Span<T> span = CollectionsMarshal.AsSpan(list);
        ref T reference = ref MemoryMarshal.GetReference(span);

        for (int i = 0; i < span.Length; i++)
        {
            if (shouldRemovePredicate.Invoke(Unsafe.Add(ref reference, i)))
            {
                list.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 转换到新类型的列表
    /// </summary>
    /// <typeparam name="TSource">原始类型</typeparam>
    /// <typeparam name="TResult">新类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="selector">选择器</param>
    /// <returns>新类型的列表</returns>
    public static List<TResult> SelectList<TSource, TResult>(this List<TSource> list, Func<TSource, TResult> selector)
    {
        Span<TSource> span = CollectionsMarshal.AsSpan(list);
        List<TResult> results = new(span.Length);

        foreach (TSource item in span)
        {
            results.Add(selector(item));
        }

        return results;
    }

    /// <summary>
    /// 按元素的键排序
    /// </summary>
    /// <typeparam name="TSource">元素类型</typeparam>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="list">列表</param>
    /// <param name="keySelector">键选择器</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void SortBy<TSource, TKey>(this List<TSource> list, Func<TSource, TKey> keySelector)
        where TKey : IComparable
    {
        list.Sort((left, right) => keySelector(left).CompareTo(keySelector(right)));
    }
}
