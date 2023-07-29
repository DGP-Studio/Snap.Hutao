// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

/// <summary>
/// 海外服/HoYoLAB 可区分
/// </summary>
[Obsolete("Use IOverseaSupportFactory instead")]
internal interface IOverseaSupport
{
    /// <summary>
    /// 是否为 海外服/HoYoLAB
    /// </summary>
    public bool IsOversea { get; }
}