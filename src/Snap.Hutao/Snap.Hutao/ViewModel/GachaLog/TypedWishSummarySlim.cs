// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 简化的类型化的祈愿概览
/// </summary>
internal sealed class TypedWishSummarySlim
{
    /// <summary>
    /// 构造一个新的简化的类型化的祈愿概览
    /// </summary>
    /// <param name="name">卡池名称</param>
    /// <param name="guaranteeOrangeThreshold">五星保底阈值</param>
    /// <param name="guaranteePurpleThreshold">四星保底阈值</param>
    public TypedWishSummarySlim(string name, int guaranteeOrangeThreshold, int guaranteePurpleThreshold)
    {
        Name = name;
        GuaranteeOrangeThreshold = guaranteeOrangeThreshold;
        GuaranteePurpleThreshold = guaranteePurpleThreshold;
    }

    /// <summary>
    /// 卡池名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 距上个五星抽数
    /// </summary>
    public int LastOrangePull { get; set; }

    /// <summary>
    /// 距上个四星抽数
    /// </summary>
    public int LastPurplePull { get; set; }

    /// <summary>
    /// 五星保底阈值
    /// </summary>
    public int GuaranteeOrangeThreshold { get; }

    /// <summary>
    /// 四星保底阈值
    /// </summary>
    public int GuaranteePurpleThreshold { get; }
}