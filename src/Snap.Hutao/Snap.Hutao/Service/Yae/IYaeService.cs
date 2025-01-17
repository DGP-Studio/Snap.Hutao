// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.Achievement;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Yae;

internal interface IYaeService
{
    ValueTask<UIAF?> GetAchievementAsync();

    ValueTask<ImmutableArray<InventoryItem>> GetInventoryAsync(CultivateProject project);
}