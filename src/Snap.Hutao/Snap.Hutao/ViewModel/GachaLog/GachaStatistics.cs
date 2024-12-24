// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed class GachaStatistics
{
    public required TypedWishSummary AvatarWish { get; init; }

    public required TypedWishSummary WeaponWish { get; init; }

    public required TypedWishSummary ChronicledWish { get; init; }

    public required TypedWishSummary StandardWish { get; init; }

    public required IAdvancedCollectionView<HistoryWish> HistoryWishes { get; init; }

    public required ImmutableArray<StatisticsItem> OrangeAvatars { get; init; }

    public required ImmutableArray<StatisticsItem> PurpleAvatars { get; init; }

    public required ImmutableArray<StatisticsItem> OrangeWeapons { get; init; }

    public required ImmutableArray<StatisticsItem> PurpleWeapons { get; init; }

    public required ImmutableArray<StatisticsItem> BlueWeapons { get; init; }
}