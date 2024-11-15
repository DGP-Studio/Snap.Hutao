// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.ViewModel.GachaLog;

[INotifyPropertyChanged]
[SuppressMessage("", "SA1124")]
internal sealed partial class TypedWishSummary : Wish
{
    public string? TypeName { get; set; }

    public string MaxOrangePullFormatted
    {
        get => SH.FormatModelBindingGachaTypedWishSummaryMaxOrangePullFormat(MaxOrangePull);
    }

    public string MinOrangePullFormatted
    {
        get => SH.FormatModelBindingGachaTypedWishSummaryMinOrangePullFormat(MinOrangePull);
    }

    public int LastOrangePull { get; set; }

    public int LastPurplePull { get; set; }

    public int GuaranteeOrangeThreshold { get; set; }

    public int GuaranteePurpleThreshold { get; set; }

    public string TotalOrangeFormatted
    {
        get => $"{TotalOrangePull} [{(TotalOrangePercent is double.NaN ? 0D : TotalOrangePercent),6:p2}]";
    }

    public string TotalPurpleFormatted
    {
        get => $"{TotalPurplePull} [{(TotalPurplePercent is double.NaN ? 0D : TotalPurplePercent),6:p2}]";
    }

    public string TotalBlueFormatted
    {
        get => $"{TotalBluePull} [{(TotalBluePercent is double.NaN ? 0D : TotalBluePercent),6:p2}]";
    }

    public string AverageOrangePullFormatted
    {
        get => SH.FormatModelBindingGachaTypedWishSummaryAveragePullFormat(AverageOrangePull);
    }

    public bool IsPredictPullAvailable { get; set => SetProperty(ref field, value); }

    public string AverageUpOrangePullFormatted
    {
        get => SH.FormatModelBindingGachaTypedWishSummaryAveragePullFormat(AverageUpOrangePull);
    }

    public string PredictedPullLeftToOrangeFormatted
    {
        get => SH.FormatViewModelGachaLogPredictedPullLeftToOrange(PredictedPullLeftToOrange, ProbabilityOfPredictedPullLeftToOrange);
    }

    public string ProbabilityOfNextPullIsOrangeFormatted
    {
        get => SH.FormatViewModelGachaLogProbabilityOfNextPullIsOrange(ProbabilityOfNextPullIsOrange);
    }

    public List<SummaryItem> OrangeList { get; set; } = default!;

    #region Internal properties for string formatting

    internal int MaxOrangePull { get; set; }

    internal int MinOrangePull { get; set; }

    internal int TotalOrangePull { get; set; }

    internal double TotalOrangePercent { get; set; }

    internal int TotalPurplePull { get; set; }

    internal double TotalPurplePercent { get; set; }

    internal int TotalBluePull { get; set; }

    internal double TotalBluePercent { get; set; }

    internal double AverageOrangePull { get; set; }

    internal double AverageUpOrangePull { get; set; }

    internal int PredictedPullLeftToOrange
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(PredictedPullLeftToOrangeFormatted));
        }
    }

    internal double ProbabilityOfPredictedPullLeftToOrange
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(PredictedPullLeftToOrangeFormatted));
        }
    }

    internal double ProbabilityOfNextPullIsOrange
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(ProbabilityOfNextPullIsOrangeFormatted));
        }
    }

    #endregion
}