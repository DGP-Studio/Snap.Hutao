// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="IEnumerable{T}"/> 扩展
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 将二维可枚举对象一维化
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>扁平的对象</returns>
    public static IEnumerable<TSource> Flatten<TSource>(this IEnumerable<IEnumerable<TSource>> source)
    {
        return source.SelectMany(x => x);
    }

    /// <summary>
    /// 计数
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TKey">计数的键类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="keySelector">键选择器</param>
    /// <returns>计数表</returns>
    public static IEnumerable<KeyValuePair<TKey, int>> CountBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource,TKey> keySelector)
        where TKey : notnull, IEquatable<TKey>
    {
        CounterInt32<TKey> counter = new();
        foreach (TSource item in source)
        {
            counter.Increase(keySelector(item));
        }

        return counter;
    }

    /// <summary>
    /// 表示一个对 <see cref="TItem"/> 类型的计数器
    /// </summary>
    /// <typeparam name="TItem">待计数的类型</typeparam>
    private class CounterInt32<TItem> : Dictionary<TItem, int>
        where TItem : notnull, IEquatable<TItem>
    {
        /// <summary>
        /// 增加计数器
        /// </summary>
        /// <param name="item">物品</param>
        public void Increase(TItem? item)
        {
            if (item != null)
            {
                if (!ContainsKey(item))
                {
                    this[item] = 0;
                }

                this[item] += 1;
            }
        }
    }
}