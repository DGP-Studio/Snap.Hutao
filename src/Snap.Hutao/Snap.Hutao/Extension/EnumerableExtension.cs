// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

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

    public static string JoinToString<T>(this IEnumerable<T> source, char separator, Action<StringBuilder, T> selector)
    {
        StringBuilder resultBuilder = new();

        IEnumerator<T> enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return string.Empty;
        }

        T first = enumerator.Current;
        selector(resultBuilder, first);

        if (!enumerator.MoveNext())
        {
            return resultBuilder.ToString();
        }

        do
        {
            resultBuilder.Append(separator);
            selector(resultBuilder, enumerator.Current);
        }
        while (enumerator.MoveNext());

        return resultBuilder.ToString();
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableReorderableDbCollection<TEntity> ToObservableReorderableDbCollection<TEntity>(this IEnumerable<TEntity> source, IServiceProvider serviceProvider)
        where TEntity : class, IReorderable
    {
        return source is List<TEntity> list
            ? new ObservableReorderableDbCollection<TEntity>(list, serviceProvider)
            : new ObservableReorderableDbCollection<TEntity>([.. source], serviceProvider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableReorderableDbCollection<TEntityOnly, TEntity> ToObservableReorderableDbCollection<TEntityOnly, TEntity>(this IEnumerable<TEntityOnly> source, IServiceProvider serviceProvider)
        where TEntityOnly : class, IEntityOnly<TEntity>
        where TEntity : class, IReorderable
    {
        return source is List<TEntityOnly> list
            ? new ObservableReorderableDbCollection<TEntityOnly, TEntity>(list, serviceProvider)
            : new ObservableReorderableDbCollection<TEntityOnly, TEntity>([.. source], serviceProvider);
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
        return string.Join(separator, collection);
    }
}