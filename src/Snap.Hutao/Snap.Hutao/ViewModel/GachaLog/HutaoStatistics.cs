// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed class HutaoStatistics
{
    public required HutaoWishSummary AvatarEvent { get; init; }

    public required HutaoWishSummary AvatarEvent2 { get; init; }

    public required HutaoWishSummary WeaponEvent { get; init; }

    public required HutaoWishSummary? Chronicled { get; init; }
}