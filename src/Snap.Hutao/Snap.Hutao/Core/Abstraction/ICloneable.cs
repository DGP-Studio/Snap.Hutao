// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Abstraction;

/// <summary>
/// 可克隆
/// </summary>
/// <typeparam name="TSelf">自身类型</typeparam>
[HighQuality]
internal interface ICloneable<out TSelf>
{
    /// <summary>
    /// 克隆
    /// </summary>
    /// <returns>新的克隆</returns>
    TSelf Clone();
}