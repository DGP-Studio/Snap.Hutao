// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.Gacha;

/// <summary>
/// 类型化的祈愿概览
/// </summary>
[HighQuality]
internal sealed class TypedWishSummary : Wish
{
    /// <summary>
    /// 最大五星抽数
    /// </summary>
    public string MaxOrangePullFormatted
    {
        get => string.Format(SH.ModelBindingGachaTypedWishSummaryMaxOrangePullFormat, MaxOrangePull);
    }

    /// <summary>
    /// 最大五星抽数
    /// </summary>
    public string MinOrangePullFormatted
    {
        get => string.Format(SH.ModelBindingGachaTypedWishSummaryMinOrangePullFormat, MinOrangePull);
    }

    /// <summary>
    /// 据上个五星抽数格式化
    /// </summary>
    public string LastOrangePullFormatted
    {
        get => string.Format(SH.ModelBindingGachaTypedWishSummaryLastPullFormat, LastOrangePull);
    }

    /// <summary>
    /// 据上个四星抽数格式化
    /// </summary>
    public string LastPurplePullFormatted
    {
        get => string.Format(SH.ModelBindingGachaTypedWishSummaryLastPullFormat, LastPurplePull);
    }

    /// <summary>
    /// 据上个五星抽数
    /// </summary>
    public int LastOrangePull { get; set; }

    /// <summary>
    /// 据上个四星抽数
    /// </summary>
    public int LastPurplePull { get; set; }

    /// <summary>
    /// 五星保底阈值
    /// </summary>
    public int GuarenteeOrangeThreshold { get; set; }

    /// <summary>
    /// 四星保底阈值
    /// </summary>
    public int GuarenteePurpleThreshold { get; set; }

    /// <summary>
    /// 五星格式化字符串
    /// </summary>
    public string TotalOrangeFormatted
    {
        get => $"{TotalOrangePull} [{TotalOrangePercent,6:p2}]";
    }

    /// <summary>
    /// 四星格式化字符串
    /// </summary>
    public string TotalPurpleFormatted
    {
        get => $"{TotalPurplePull} [{TotalPurplePercent,6:p2}]";
    }

    /// <summary>
    /// 三星格式化字符串
    /// </summary>
    public string TotalBlueFormatted
    {
        get => $"{TotalBluePull} [{TotalBluePercent,6:p2}]";
    }

    /// <summary>
    /// 平均五星抽数
    /// </summary>
    public string AverageOrangePullFormatted
    {
        get => string.Format(SH.ModelBindingGachaTypedWishSummaryAveragePullFormat, AverageOrangePull);
    }

    /// <summary>
    /// 平均Up五星抽数
    /// </summary>
    public string AverageUpOrangePullFormatted
    {
        get => string.Format(SH.ModelBindingGachaTypedWishSummaryAveragePullFormat, AverageUpOrangePull);
    }

    /// <summary>
    /// 五星列表
    /// </summary>
    public List<SummaryItem> OrangeList { get; set; } = default!;

    /// <summary>
    /// 最大五星抽数
    /// </summary>
    internal int MaxOrangePull { get; set; }

    /// <summary>
    /// 最小五星抽数
    /// </summary>
    internal int MinOrangePull { get; set; }

    /// <summary>
    /// 五星总数
    /// </summary>
    internal int TotalOrangePull { get; set; }

    /// <summary>
    /// 五星总百分比
    /// </summary>
    internal double TotalOrangePercent { get; set; }

    /// <summary>
    /// 四星总数
    /// </summary>
    internal int TotalPurplePull { get; set; }

    /// <summary>
    /// 四星总百分比
    /// </summary>
    internal double TotalPurplePercent { get; set; }

    /// <summary>
    /// 三星总数
    /// </summary>
    internal int TotalBluePull { get; set; }

    /// <summary>
    /// 三星总百分比
    /// </summary>
    internal double TotalBluePercent { get; set; }

    /// <summary>
    /// 平均五星抽数
    /// </summary>
    internal double AverageOrangePull { get; set; }

    /// <summary>
    /// 平均Up五星抽数
    /// </summary>
    internal double AverageUpOrangePull { get; set; }
}