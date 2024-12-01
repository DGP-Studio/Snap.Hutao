// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed partial class ResinStatisticsItem : ObservableObject
{
    private readonly bool isCondensedResinAvailable;
    private readonly ResinStatisticsItemKind kind;

    public ResinStatisticsItem(string title, ResinStatisticsItemKind kind, int resinPerBlossom, bool isCondensedResinAvailable)
    {
        this.kind = kind;
        this.isCondensedResinAvailable = isCondensedResinAvailable;
        Title = title;
        ResinPerBlossom = resinPerBlossom;
        SelectedWorldDropProability = MaterialDropDistribution.Nine;
    }

    public string Title { get; }

    public MaterialDropDistribution SelectedWorldDropProability
    {
        get;
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(TotalResin));
                OnPropertyChanged(nameof(CondensedResin));
                OnPropertyChanged(nameof(Days));
            }
        }
    }

    public int ResinPerBlossom { get; }

    public double RawItemCount { get; set; }

    public bool HasData
    {
        get => RawItemCount > 0D;
    }

    public int TotalResin
    {
        get => RawTimes * ResinPerBlossom;
    }

    public int? CondensedResin
    {
        get => isCondensedResinAvailable ? TotalResin / 40 : null;
    }

    public string Days
    {
        get => $"还需 {TotalResin / 200D:F1} 天";
    }

    internal int RawTimes
    {
        get
        {
            double expectation = kind switch
            {
                ResinStatisticsItemKind.BlossomOfWealth => SelectedWorldDropProability.BlossomOfWealth,
                ResinStatisticsItemKind.BlossomOfRevelation => SelectedWorldDropProability.BlossomOfRevelation,
                ResinStatisticsItemKind.TalentBooks => SelectedWorldDropProability.TalentBooks,
                ResinStatisticsItemKind.WeaponAscension => SelectedWorldDropProability.WeaponAscension,
                ResinStatisticsItemKind.NormalBoss => SelectedWorldDropProability.NormalBoss,
                ResinStatisticsItemKind.WeeklyBoss => SelectedWorldDropProability.WeeklyBoss,
                _ => throw HutaoException.NotSupported(),
            };

            return (int)Math.Ceiling(RawItemCount / expectation);
        }
    }
}