// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model.Metadata.Item;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed partial class ResinStatistics : ObservableObject
{
    public ResinStatistics()
    {
        int worldLevel = LocalSetting.Get(SettingKeys.ResinStatisticsSelectedDropDistribution, MaterialDropDistribution.Nine.WorldLevel);
        SelectedDropDistribution = DropDistributions.Single(d => d.WorldLevel == worldLevel);
    }

    [UsedImplicitly]
    public ImmutableArray<MaterialDropDistribution> DropDistributions { get; } = MaterialDropDistribution.Distributions;

    [AllowNull]
    public MaterialDropDistribution SelectedDropDistribution
    {
        get;
        set
        {
            if (value is null)
            {
                return;
            }

            field = value;
            LocalSetting.Set(SettingKeys.ResinStatisticsSelectedDropDistribution, value.WorldLevel);
            BlossomOfRevelation.SelectedDropDistribution = value;
            TalentAscension.SelectedDropDistribution = value;
            WeaponAscension.SelectedDropDistribution = value;
            NormalBoss.SelectedDropDistribution = value;
            WeeklyBoss.SelectedDropDistribution = value;

            RefreshBlossomOfWealth();
            BlossomOfWealth.SelectedDropDistribution = value;
            OnPropertyChanged(nameof(ItemsSource));
        }
    }

    public ResinStatisticsItem BlossomOfWealth { get; } = new(SH.ViewModelCultivationResinStatisticsBlossomOfWealthTitle, ResinStatisticsItemKind.BlossomOfWealth, 20);

    public ResinStatisticsItem BlossomOfRevelation { get; } = new(SH.ViewModelCultivationResinStatisticsBlossomOfRevelationTitle, ResinStatisticsItemKind.BlossomOfRevelation, 20);

    public ResinStatisticsItem TalentAscension { get; } = new(SH.ViewModelCultivationResinStatisticsTalentAscensionTitle, ResinStatisticsItemKind.TalentAscension, 20);

    public ResinStatisticsItem WeaponAscension { get; } = new(SH.ViewModelCultivationResinStatisticsWeaponAscensionTitle, ResinStatisticsItemKind.WeaponAscension, 20);

    public ResinStatisticsItem NormalBoss { get; } = new(SH.ViewModelCultivationResinStatisticsNormalBossTitle, ResinStatisticsItemKind.NormalBoss, 40);

    public ResinStatisticsItem WeeklyBoss { get; } = new(SH.ViewModelCultivationResinStatisticsWeeklyBossTitle, ResinStatisticsItemKind.WeeklyBoss, 60);

    public IEnumerable<ResinStatisticsItem> ItemsSource
    {
        get
        {
            if (BlossomOfWealth.HasData)
            {
                yield return BlossomOfWealth;
            }

            if (BlossomOfRevelation.HasData)
            {
                yield return BlossomOfRevelation;
            }

            if (TalentAscension.HasData)
            {
                yield return TalentAscension;
            }

            if (WeaponAscension.HasData)
            {
                yield return WeaponAscension;
            }

            if (NormalBoss.HasData)
            {
                yield return NormalBoss;
            }

            if (WeeklyBoss.HasData)
            {
                yield return WeeklyBoss;
            }
        }
    }

    public void RefreshBlossomOfWealth()
    {
        BlossomOfWealth.MiscMoraEarned =
            BlossomOfRevelation.Mora +
            TalentAscension.Mora +
            WeaponAscension.Mora +
            NormalBoss.Mora +
            WeeklyBoss.Mora;
    }
}