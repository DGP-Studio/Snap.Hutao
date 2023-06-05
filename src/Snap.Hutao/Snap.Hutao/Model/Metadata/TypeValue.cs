// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 类型与值
/// </summary>
/// <typeparam name="TType">类型</typeparam>
/// <typeparam name="TValue">值</typeparam>
internal class TypeValue<TType, TValue>
{
    /// <summary>
    /// 构造一个新的类型与值
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="value">值</param>
    public TypeValue(TType type, TValue value)
    {
        Type = type;
        Value = value;
    }

    /// <summary>
    /// 类型
    /// </summary>
    public TType Type { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public TValue Value { get; set; }
}