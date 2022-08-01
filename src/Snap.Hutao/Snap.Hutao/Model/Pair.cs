// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

/// <summary>
/// 对
/// </summary>
/// <typeparam name="TKey">键的类型</typeparam>
/// <typeparam name="TValue">值的类型</typeparam>
public class Pair<TKey, TValue>
{
    /// <summary>
    /// 构造一个新的对
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public Pair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public TValue Value { get; set; }
}
