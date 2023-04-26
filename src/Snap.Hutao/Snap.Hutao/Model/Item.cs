// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model;

/// <summary>
/// 物品基类
/// </summary>
[HighQuality]
internal class Item : INameIcon
{
    /// <summary>
    /// 物品名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 主图标
    /// </summary>
    public Uri Icon { get; set; } = default!;

    /// <summary>
    /// 小图标
    /// </summary>
    public Uri Badge { get; set; } = default!;

    /// <summary>
    /// 星级
    /// </summary>
    public ItemQuality Quality { get; set; }
}