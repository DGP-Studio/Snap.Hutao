// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

/// <summary>
/// 对
/// </summary>
/// <typeparam name="TKey">键的类型</typeparam>
/// <typeparam name="TValue1">值1的类型</typeparam>
/// <typeparam name="TValue2">值2的类型</typeparam>
public class Pair2<TKey, TValue1, TValue2>
{
    /// <summary>
    /// 构造一个新的对
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    public Pair2(TKey key, TValue1 value1, TValue2 value2)
    {
        Key = key;
        Value1 = value1;
        Value2 = value2;
    }

    /// <summary>
    /// 键
    /// </summary>
    public TKey Key { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public TValue1 Value1 { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public TValue2 Value2 { get; set; }
}