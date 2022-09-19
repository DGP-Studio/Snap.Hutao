// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha.Abstraction;

namespace Snap.Hutao.Model.Binding.Gacha;

/// <summary>
/// 类型化的祈愿概览
/// </summary>
public class TypedWishSummary : WishBase
{
    /// <summary>
    /// 最大五星抽数
    /// </summary>
    public int MaxOrangePull { get; set; }

    /// <summary>
    /// 最大五星抽数
    /// </summary>
    public string MaxOrangePullFormatted
    {
        get => $"最非 {MaxOrangePull} 抽";
    }

    /// <summary>
    /// 最小五星抽数
    /// </summary>
    public int MinOrangePull { get; set; }

    /// <summary>
    /// 最大五星抽数
    /// </summary>
    public string MinOrangePullFormatted
    {
        get => $"最欧 {MinOrangePull} 抽";
    }

    /// <summary>
    /// 据上个五星抽数
    /// </summary>
    public int LastOrangePull { get; set; }

    /// <summary>
    /// 五星保底阈值
    /// </summary>
    public int GuarenteeOrangeThreshold { get; set; }

    /// <summary>
    /// 据上个四星抽数
    /// </summary>
    public int LastPurplePull { get; set; }

    /// <summary>
    /// 四星保底阈值
    /// </summary>
    public int GuarenteePurpleThreshold { get; set; }

    /// <summary>
    /// 五星总数
    /// </summary>
    public int TotalOrangePull { get; set; }

    /// <summary>
    /// 五星总百分比
    /// </summary>
    public double TotalOrangePercent { get; set; }

    /// <summary>
    /// 五星格式化字符串
    /// </summary>
    public string TotalOrangeFormatted
    {
        get => $"{TotalOrangePull} [{TotalOrangePercent,6:p2}]";
    }

    /// <summary>
    /// 四星总数
    /// </summary>
    public int TotalPurplePull { get; set; }

    /// <summary>
    /// 四星总百分比
    /// </summary>
    public double TotalPurplePercent { get; set; }

    /// <summary>
    /// 四星格式化字符串
    /// </summary>
    public string TotalPurpleFormatted
    {
        get => $"{TotalPurplePull} [{TotalPurplePercent,6:p2}]";
    }

    /// <summary>
    /// 三星总数
    /// </summary>
    public int TotalBluePull { get; set; }

    /// <summary>
    /// 三星总百分比
    /// </summary>
    public double TotalBluePercent { get; set; }

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
    public double AverageOrangePull { get; set; }

    /// <summary>
    /// 平均五星抽数
    /// </summary>
    public string AverageOrangePullFormatted
    {
        get => $"{AverageOrangePull:f2} 抽";
    }

    /// <summary>
    /// 平均Up五星抽数
    /// </summary>
    public double AverageUpOrangePull { get; set; }

    /// <summary>
    /// 平均Up五星抽数
    /// </summary>
    public string AverageUpOrangePullFormatted
    {
        get => $"{AverageUpOrangePull:f2} 抽";
    }

    /// <summary>
    /// 五星列表
    /// </summary>
    public List<SummaryItem> OrangeList { get; set; } = default!;
}