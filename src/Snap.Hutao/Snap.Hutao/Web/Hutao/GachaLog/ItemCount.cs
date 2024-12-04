// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.GachaLog;

/// <summary>
/// 物品与个数
/// </summary>
internal sealed class ItemCount
{
    /// <summary>
    /// 物品 Id
    /// </summary>
    public uint Item { get; set; }

    /// <summary>
    /// 个数
    /// </summary>
    public uint Count { get; set; }
}