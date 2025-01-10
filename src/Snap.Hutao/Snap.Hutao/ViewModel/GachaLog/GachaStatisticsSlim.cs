// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed class GachaStatisticsSlim
{
    public required string Uid { get; init; }

    public required TypedWishSummarySlim AvatarWish { get; init; }

    public required TypedWishSummarySlim WeaponWish { get; init; }

    public required TypedWishSummarySlim StandardWish { get; init; }
}