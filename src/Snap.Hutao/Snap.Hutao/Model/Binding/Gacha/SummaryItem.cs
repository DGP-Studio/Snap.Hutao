// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha.Abstraction;

namespace Snap.Hutao.Model.Binding.Gacha;

/// <summary>
/// 祈愿卡池列表物品
/// </summary>
public class SummaryItem : ItemBase
{
    /// <summary>
    /// 据上次
    /// </summary>
    public int LastPull { get; set; }

    /// <summary>
    /// 是否为Up物品
    /// </summary>
    public bool IsUp { get; set; }

    /// <summary>
    /// 是否为大保底
    /// </summary>
    public bool IsGuarentee { get; set; }

    /// <summary>
    /// 获取时间
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 获取时间
    /// </summary>
    public string TimeFormatted
    {
        get => $"{Time:yyy.MM.dd HH:mm:ss}";
    }

    /// <summary>
    /// 颜色
    /// </summary>
    public Windows.UI.Color Color { get; set; }
}
