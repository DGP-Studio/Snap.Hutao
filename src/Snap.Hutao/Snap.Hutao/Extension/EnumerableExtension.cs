// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="IEnumerable{T}"/> 扩展
/// </summary>
[HighQuality]
internal static partial class EnumerableExtension
{
    public static TElement? ElementAtOrLastOrDefault<TElement>(this IEnumerable<TElement> source, int index)
    {
        return source.ElementAtOrDefault(index) ?? source.LastOrDefault();
    }

    /// <summary>
    /// 如果传入集合不为空则原路返回，
    /// 如果传入集合为空返回一个集合的空集
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>源集合或空集</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource>? source)
    {
        return source ?? Enumerable.Empty<TSource>();
    }

    /// <summary>
    /// 将源转换为仅包含单个元素的枚举
    /// </summary>
    /// <typeparam name="TSource">源的类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>集合</returns>
#if NET8_0
    [Obsolete("Use C# 12 Collection Literal instead")]
#endif
    public static IEnumerable<TSource> Enumerate<TSource>(this TSource source)
    {
        yield return source;
    }

    /// <summary>
    /// 寻找枚举中唯一的值,找不到时
    /// 回退到首个或默认值
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="predicate">谓语</param>
    /// <returns>目标项</returns>
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

    /// <summary>
    /// 转换到 <see cref="ObservableCollection{T}"/>
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="source">源</param>
    /// <returns><see cref="ObservableCollection{T}"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }

    /// <summary>
    /// Concatenates each element from the collection into single string.
    /// </summary>
    /// <typeparam name="T">Type of array elements.</typeparam>
    /// <param name="collection">Collection to convert. Cannot be <see langword="null"/>.</param>
    /// <param name="separator">Delimiter between elements in the final string.</param>
    /// <returns>Converted collection into string.</returns>
    public static string ToString<T>(this IEnumerable<T> collection, char separator)
    {
        string result = string.Join(separator, collection);
        return result.Length > 0 ? result : string.Empty;
    }
}