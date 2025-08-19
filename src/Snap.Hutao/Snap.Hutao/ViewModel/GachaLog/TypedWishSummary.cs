// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed partial class TypedWishSummary : Wish, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public required string TypeName { get; init; }

    public string FormattedMaxOrangePull
    {
        get => SH.FormatModelBindingGachaTypedWishSummaryMaxOrangePull(MaxOrangePull);
    }

    public string FormattedMinOrangePull
    {
        get => SH.FormatModelBindingGachaTypedWishSummaryMinOrangePull(MinOrangePull);
    }

    public required int LastOrangePull { get; init; }

    public required int LastPurplePull { get; init; }

    public required int GuaranteeOrangeThreshold { get; init; }

    public required int GuaranteePurpleThreshold { get; init; }

    public string FormattedTotalOrange
    {
        get => $"{TotalOrangePull} [{(TotalOrangePercent is double.NaN ? 0D : TotalOrangePercent),6:p2}]";
    }

    public string FormattedTotalPurple
    {
        get => $"{TotalPurplePull} [{(TotalPurplePercent is double.NaN ? 0D : TotalPurplePercent),6:p2}]";
    }

    public string FormattedTotalBlue
    {
        get => $"{TotalBluePull} [{(TotalBluePercent is double.NaN ? 0D : TotalBluePercent),6:p2}]";
    }

    public string FormattedAverageOrangePull
    {
        get => SH.FormatModelBindingGachaTypedWishSummaryAveragePull(AverageOrangePull);
    }

    public bool IsPredictPullAvailable { get; set => SetProperty(ref field, value); }

    public string FormattedAverageUpOrangePull
    {
        get => SH.FormatModelBindingGachaTypedWishSummaryAveragePull(AverageUpOrangePull);
    }

    public string FormattedPredictedPullLeftToOrange
    {
        get => SH.FormatViewModelGachaLogPredictedPullLeftToOrange(PredictedPullLeftToOrange, ProbabilityOfPredictedPullLeftToOrange);
    }

    public string FormattedProbabilityOfNextPullIsOrange
    {
        get => SH.FormatViewModelGachaLogProbabilityOfNextPullIsOrange(ProbabilityOfNextPullIsOrange);
    }

    public required List<SummaryItem> OrangeList { get; init; }

    internal required int MaxOrangePull { get; init; }

    internal required int MinOrangePull { get; init; }

    internal required int TotalOrangePull { get; init; }

    internal required double TotalOrangePercent { get; init; }

    internal required int TotalPurplePull { get; init; }

    internal required double TotalPurplePercent { get; init; }

    internal required int TotalBluePull { get; init; }

    internal required double TotalBluePercent { get; init; }

    internal required double AverageOrangePull { get; init; }

    internal required double AverageUpOrangePull { get; init; }

    internal int PredictedPullLeftToOrange
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(FormattedPredictedPullLeftToOrange));
        }
    }

    internal double ProbabilityOfPredictedPullLeftToOrange
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(FormattedPredictedPullLeftToOrange));
        }
    }

    internal double ProbabilityOfNextPullIsOrange
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(FormattedProbabilityOfNextPullIsOrange));
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        return false;
    }
}