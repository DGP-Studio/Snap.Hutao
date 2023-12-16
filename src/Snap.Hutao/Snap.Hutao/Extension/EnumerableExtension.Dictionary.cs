// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="Dictionary{TKey, TValue}"/> 部分
/// </summary>
internal static partial class EnumerableExtension
{
    public static bool IsNullOrEmpty<TKey, TValue>([NotNullWhen(false)] this Dictionary<TKey, TValue>? source)
        where TKey : notnull
    {
        if (source is { Count: > 0 })
        {
            return false;
        }

        return true;
    }

    public static void IncreaseOne<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        where TKey : notnull
        where TValue : struct, IIncrementOperators<TValue>
    {
        ++CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
    }

    public static void IncreaseValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        where TKey : notnull
        where TValue : struct, IAdditionOperators<TValue, TValue, TValue>
    {
        // ref the value, so that we can manipulate it outside the dict.
        ref TValue current = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
        current += value;
    }

    public static bool TryIncreaseOne<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        where TKey : notnull
        where TValue : struct, IIncrementOperators<TValue>
    {
        ref TValue value = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        if (!Unsafe.IsNullRef(ref value))
        {
            ++value;
            return true;
        }

        return false;
    }

    public static Dictionary<TKey, TSource> ToDictionaryIgnoringDuplicateKeys<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        where TKey : notnull
    {
        Dictionary<TKey, TSource> dictionary = [];

        foreach (TSource value in source)
        {
            dictionary[keySelector(value)] = value;
        }

        return dictionary;
    }

    public static Dictionary<TKey, TValue> ToDictionaryIgnoringDuplicateKeys<TKey, TValue, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
        where TKey : notnull
    {
        Dictionary<TKey, TValue> dictionary = [];

        foreach (TSource value in source)
        {
            dictionary[keySelector(value)] = elementSelector(value);
        }

        return dictionary;
    }
}
