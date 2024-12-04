// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Web.Hutao.GachaLog;

/// <summary>
/// 服务器接口使用的祈愿记录物品
/// </summary>
internal sealed class GachaItem
{
    /// <summary>
    /// 祈愿记录分类
    /// </summary>
    public GachaType GachaType { get; set; }

    /// <summary>
    /// 祈愿记录查询分类
    /// 合并保底的卡池使用此属性
    /// 仅4种（不含400）
    /// </summary>
    public GachaType QueryType { get; set; }

    /// <summary>
    /// 物品Id
    /// </summary>
    public uint ItemId { get; set; }

    /// <summary>
    /// 获取时间
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }
}