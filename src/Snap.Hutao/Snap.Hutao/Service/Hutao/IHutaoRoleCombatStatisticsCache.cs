// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.Complex;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Hutao;

internal interface IHutaoRoleCombatStatisticsCache
{
    int RecordTotal { get; }

    ImmutableArray<AvatarView> AvatarAppearances { get; }

    ValueTask InitializeForRoleCombatViewAsync(HutaoRoleCombatStatisticsMetadataContext context);
}