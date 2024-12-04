// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.GachaLog;

internal sealed class GachaEntry
{
    /// <summary>
    /// Uid
    /// </summary>
    public string Uid { get; set; } = default!;

    /// <summary>
    /// 是否被排除出了全球统计
    /// </summary>
    public bool Excluded { get; set; }

    /// <summary>
    /// 物品个数
    /// </summary>
    public int ItemCount { get; set; }
}