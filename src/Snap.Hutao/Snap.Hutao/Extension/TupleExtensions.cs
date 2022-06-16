// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Extension;

/// <summary>
/// 元组扩展
/// </summary>
public static class TupleExtensions
{
    /// <summary>
    /// 将二项元组转化为一个单项的字典
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="tuple">元组</param>
    /// <returns>仅包含一个项的字典</returns>
    public static IDictionary<TKey, TValue> AsDictionary<TKey, TValue>(this (TKey Key, TValue Value) tuple)
        where TKey : notnull
    {
        return new Dictionary<TKey, TValue>(1) { { tuple.Key, tuple.Value } };
    }
}
