// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Abstraction;

/// <summary>
/// 指示该类可以解构为元组
/// </summary>
/// <typeparam name="T1">元组的第一个类型</typeparam>
/// <typeparam name="T2">元组的第二个类型</typeparam>
[HighQuality]
internal interface IDeconstructable<T1, T2>
{
    /// <summary>
    /// 解构
    /// </summary>
    /// <param name="t1">第一个元素</param>
    /// <param name="t2">第二个元素</param>
    void Deconstruct(out T1 t1, out T2 t2);
}