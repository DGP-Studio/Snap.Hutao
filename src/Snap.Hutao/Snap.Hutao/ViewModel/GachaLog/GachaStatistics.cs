// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed class GachaStatistics
{
    public TypedWishSummary AvatarWish { get; set; } = default!;

    public TypedWishSummary WeaponWish { get; set; } = default!;

    public TypedWishSummary ChronicledWish { get; set; } = default!;

    public TypedWishSummary StandardWish { get; set; } = default!;

    public AdvancedCollectionView<HistoryWish> HistoryWishes { get; set; } = default!;

    public List<StatisticsItem> OrangeAvatars { get; set; } = default!;

    public List<StatisticsItem> PurpleAvatars { get; set; } = default!;

    public List<StatisticsItem> OrangeWeapons { get; set; } = default!;

    public List<StatisticsItem> PurpleWeapons { get; set; } = default!;

    public List<StatisticsItem> BlueWeapons { get; set; } = default!;
}