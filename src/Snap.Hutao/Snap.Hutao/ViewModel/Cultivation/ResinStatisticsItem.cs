// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed partial class ResinStatisticsItem
{
    private readonly bool canUseCondensedResin;
    private readonly ResinStatisticsItemKind kind;

    public ResinStatisticsItem(string title, ResinStatisticsItemKind kind, int resinPerBlossom, bool canUseCondensedResin)
    {
        this.kind = kind;
        this.canUseCondensedResin = canUseCondensedResin;
        Title = title;
        ResinPerBlossom = resinPerBlossom;
        SelectedDropDistribution = MaterialDropDistribution.Nine;
    }

    public string Title { get; }

    public int ResinPerBlossom { get; }

    [AllowNull]
    public MaterialDropDistribution SelectedDropDistribution
    {
        get;
        set
        {
            if (value is not null)
            {
                field = value;
            }
        }
    }

    public double RawItemCount { get; set; }

    [UsedImplicitly]
    public bool HasData
    {
        get => ItemCount > 0D;
    }

    public int TotalResin
    {
        get => RawTimes * ResinPerBlossom;
    }

    public int? CondensedResin
    {
        get => canUseCondensedResin ? (int)Math.Ceiling(TotalResin / 40D) : null;
    }

    public string Days
    {
        get => SH.FormatViewModelCultivationResinStatisticsItemRemainDays((int)Math.Ceiling(TotalResin / (1440D / 8)));
    }

    /// <summary>
    /// ONLY BlossomOfWealth has value.
    /// </summary>
    internal double MiscMoraEarned { get; set; }

    internal int RawTimes
    {
        get
        {
            double expectation = kind switch
            {
                ResinStatisticsItemKind.BlossomOfWealth => SelectedDropDistribution.BlossomOfWealth,
                ResinStatisticsItemKind.BlossomOfRevelation => SelectedDropDistribution.BlossomOfRevelation,
                ResinStatisticsItemKind.TalentAscension => SelectedDropDistribution.TalentBooks,
                ResinStatisticsItemKind.WeaponAscension => SelectedDropDistribution.WeaponAscension,
                ResinStatisticsItemKind.NormalBoss => SelectedDropDistribution.NormalBoss,
                ResinStatisticsItemKind.WeeklyBoss => SelectedDropDistribution.WeeklyBoss,
                _ => throw HutaoException.NotSupported(),
            };

            return (int)Math.Ceiling(ItemCount / expectation);
        }
    }

    internal double Mora
    {
        get
        {
            if (kind is ResinStatisticsItemKind.BlossomOfWealth)
            {
                throw HutaoException.NotSupported();
            }

            double expectation = kind switch
            {
                ResinStatisticsItemKind.TalentAscension => SelectedDropDistribution.TalentBooksMora,
                ResinStatisticsItemKind.WeaponAscension => SelectedDropDistribution.WeaponAscensionMora,
                ResinStatisticsItemKind.NormalBoss => SelectedDropDistribution.NormalBossMora,
                ResinStatisticsItemKind.WeeklyBoss => SelectedDropDistribution.WeeklyBossMora,
                _ => 0D,
            };

            return (RawTimes * expectation) + (TotalResin * MaterialDropDistribution.MoraPerResin);
        }
    }

    internal double ItemCount
    {
        get => RawItemCount - MiscMoraEarned;
    }
}