// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 祈愿卡池列表物品
/// </summary>
[HighQuality]
internal sealed class SummaryItem : Item
{
    /// <summary>
    /// 是否为Up物品
    /// </summary>
    public bool IsUp { get; set; }

    /// <summary>
    /// 是否为大保底
    /// </summary>
    public bool IsGuarantee { get; set; }

    /// <summary>
    /// 五星保底阈值
    /// </summary>
    public int GuaranteeOrangeThreshold { get; set; }

    /// <summary>
    /// 据上次
    /// </summary>
    public int LastPull { get; set; }

    /// <summary>
    /// 获取时间
    /// </summary>
    public string TimeFormatted
    {
        get => $"{Time.ToLocalTime():yyy.MM.dd HH:mm:ss}";
    }

    /// <summary>
    /// 颜色
    /// </summary>
    public Windows.UI.Color Color { get; set; }

    /// <summary>
    /// 获取时间
    /// </summary>
    internal DateTimeOffset Time { get; set; }
}