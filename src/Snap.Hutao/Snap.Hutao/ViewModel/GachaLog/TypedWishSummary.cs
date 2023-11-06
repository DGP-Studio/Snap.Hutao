// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 类型化的祈愿概览
/// </summary>
[HighQuality]
[INotifyPropertyChanged]
[SuppressMessage("", "SA1124")]
internal sealed partial class TypedWishSummary : Wish
{
    private bool isPredictPullAvailable;
    private int predictedPullLeftToOrange;
    private double probabilityOfPredictedPullLeftToOrange;
    private double probabilityOfNextPullIsOrange;

    /// <summary>
    /// 类型名称，不受语言影响
    /// </summary>
    public string? TypeName { get; set; }

    /// <summary>
    /// 最大五星抽数
    /// </summary>
    public string MaxOrangePullFormatted
    {
        get => SH.ModelBindingGachaTypedWishSummaryMaxOrangePullFormat.Format(MaxOrangePull);
    }

    /// <summary>
    /// 最大五星抽数
    /// </summary>
    public string MinOrangePullFormatted
    {
        get => SH.ModelBindingGachaTypedWishSummaryMinOrangePullFormat.Format(MinOrangePull);
    }

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
    public int GuaranteeOrangeThreshold { get; set; }

    /// <summary>
    /// 四星保底阈值
    /// </summary>
    public int GuaranteePurpleThreshold { get; set; }

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
        get => SH.ModelBindingGachaTypedWishSummaryAveragePullFormat.Format(AverageOrangePull);
    }

    /// <summary>
    /// 抽数预测是否可用
    /// </summary>
    public bool IsPredictPullAvailable { get => isPredictPullAvailable; set => SetProperty(ref isPredictPullAvailable, value); }

    /// <summary>
    /// 平均Up五星抽数
    /// </summary>
    public string AverageUpOrangePullFormatted
    {
        get => SH.ModelBindingGachaTypedWishSummaryAveragePullFormat.Format(AverageUpOrangePull);
    }

    /// <summary>
    /// 预计出金的抽数与概率
    /// </summary>
    public string PredictedPullLeftToOrangeFormatted
    {
        get => SH.ViewModelGachaLogPredictedPullLeftToOrange.Format(PredictedPullLeftToOrange, ProbabilityOfPredictedPullLeftToOrange);
    }

    /// <summary>
    /// 预计出金的抽数与概率
    /// </summary>
    public string ProbabilityOfNextPullIsOrangeFormatted
    {
        get => SH.ViewModelGachaLogProbabilityOfNextPullIsOrange.Format(ProbabilityOfNextPullIsOrange);
    }

    /// <summary>
    /// 五星列表
    /// </summary>
    public List<SummaryItem> OrangeList { get; set; } = default!;

    #region Internal properties for string formatting

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

    /// <summary>
    /// 预测的x抽后出金
    /// </summary>
    internal int PredictedPullLeftToOrange
    {
        get => predictedPullLeftToOrange;
        set
        {
            predictedPullLeftToOrange = value;
            OnPropertyChanged(nameof(PredictedPullLeftToOrangeFormatted));
        }
    }

    /// <summary>
    /// 预测的x抽后出金概率
    /// </summary>
    internal double ProbabilityOfPredictedPullLeftToOrange
    {
        get => probabilityOfPredictedPullLeftToOrange;
        set
        {
            probabilityOfPredictedPullLeftToOrange = value;
            OnPropertyChanged(nameof(PredictedPullLeftToOrangeFormatted));
        }
    }

    /// <summary>
    /// 下抽出金的概率
    /// </summary>
    internal double ProbabilityOfNextPullIsOrange
    {
        get => probabilityOfNextPullIsOrange;
        set
        {
            probabilityOfNextPullIsOrange = value;
            OnPropertyChanged(nameof(ProbabilityOfNextPullIsOrangeFormatted));
        }
    }
    #endregion
}