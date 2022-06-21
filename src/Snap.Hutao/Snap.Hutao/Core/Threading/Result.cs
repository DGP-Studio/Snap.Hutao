// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 用于包装异步操作的结果
/// </summary>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TValue"></typeparam>
public record Result<TResult, TValue>
    where TResult : notnull
    where TValue : notnull
{
    /// <summary>
    /// 构造一个新的结果
    /// </summary>
    /// <param name="isOk">是否成功</param>
    /// <param name="value">值</param>
    public Result(TResult isOk, TValue value)
    {
        IsOk = isOk;
        Value = value;
    }

    /// <summary>
    /// 是否成功
    /// </summary>
    public TResult IsOk { get; }

    /// <summary>
    /// 值
    /// </summary>
    public TValue Value { get; }

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
