// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="Dictionary{TKey, TValue}"/> 部分
/// </summary>
public static partial class EnumerableExtension
{
    /// <summary>
    /// 获取值或默认值
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>结果值</returns>
    public static TValue? GetValueOrDefault2<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        return defaultValue;
    }

    /// <summary>
    /// 增加计数
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="dict">字典</param>
    /// <param name="key">键</param>
    public static void Increase<TKey>(this Dictionary<TKey, int> dict, TKey key)
        where TKey : notnull
    {
        ++CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
    }

    /// <summary>
    /// 增加计数
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="dict">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">增加的值</param>
    public static void Increase<TKey>(this Dictionary<TKey, int> dict, TKey key, int value)
        where TKey : notnull
    {
        // ref the value, so that we can manipulate it outside the dict.
        ref int current = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
        current += value;
    }

    /// <summary>
    /// 增加计数
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="dict">字典</param>
    /// <param name="key">键</param>
    /// <returns>是否存在键值</returns>
    public static bool TryIncrease<TKey>(this Dictionary<TKey, int> dict, TKey key)
        where TKey : notnull
    {
        ref int value = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        if (!Unsafe.IsNullRef(ref value))
        {
            ++value;
            return true;
        }

        return false;
    }

    /// <inheritdoc cref="Enumerable.ToDictionary{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    public static Dictionary<TKey, TSource> ToDictionaryOverride<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        where TKey : notnull
    {
        Dictionary<TKey, TSource> dictionary = new();

        foreach (TSource value in source)
        {
            dictionary[keySelector(value)] = value;
        }

        return dictionary;
    }

    /// <inheritdoc cref="Enumerable.ToDictionary{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement})"/>
    public static Dictionary<TKey, TValue> ToDictionaryOverride<TKey, TValue, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        where TKey : notnull
    {
        Dictionary<TKey, TValue> dictionary = new();

        foreach (TSource value in source)
        {
            dictionary[keySelector(value)] = valueSelector(value);
        }

        return dictionary;
    }
}
