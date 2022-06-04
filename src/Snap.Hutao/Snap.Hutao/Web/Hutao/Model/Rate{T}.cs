// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 比率
/// </summary>
/// <typeparam name="T"><see cref="Id"/> 的类型</typeparam>
public class Rate<T>
{
    /// <summary>
    /// 表示唯一标识符的实例
    /// </summary>
    public T Id { get; set; } = default!;

    /// <summary>
    /// 比率
    /// </summary>
    public decimal Value { get; set; }
}