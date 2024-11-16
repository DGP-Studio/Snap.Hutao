// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.Complex;

namespace Snap.Hutao.Service.Hutao;

internal interface IHutaoRoleCombatStatisticsCache
{
    int RecordTotal { get; set; }

    List<AvatarView>? AvatarAppearances { get; set; }

    ValueTask<bool> InitializeForRoleCombatViewAsync();
}