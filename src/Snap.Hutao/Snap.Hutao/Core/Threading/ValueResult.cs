// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 用于包装异步操作的结果
/// </summary>
/// <typeparam name="TResult">结果类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
internal readonly struct ValueResult<TResult, TValue> : IDeconstructable<TResult, TValue>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public readonly TResult IsOk;

    /// <summary>
    /// 值
    /// </summary>
    public readonly TValue Value;

    /// <summary>
    /// 构造一个新的结果
    /// </summary>
    /// <param name="isOk">是否成功</param>
    /// <param name="value">值</param>
    public ValueResult(TResult isOk, TValue value)
    {
        IsOk = isOk;
        Value = value;
    }

    /// <summary>
    /// 用于元组析构
    /// </summary>
    /// <param name="isOk">是否成功</param>
    /// <param name="value">值</param>
    public void Deconstruct(out TResult isOk, out TValue value)
    {
        isOk = IsOk;
        value = Value;
    }
}