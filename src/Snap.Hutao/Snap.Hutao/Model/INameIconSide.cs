// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

/// <summary>
/// 包括侧面图标的名称与图标
/// </summary>
[HighQuality]
internal interface INameIconSide : INameIcon
{
    /// <summary>
    /// 侧面图标
    /// </summary>
    Uri SideIcon { get; }
}