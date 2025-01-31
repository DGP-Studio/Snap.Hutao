// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.InterChange.Inventory;

namespace Snap.Hutao.Service.Yae;

internal interface IYaeService
{
    ValueTask<UIAF?> GetAchievementAsync();

    ValueTask<UIIF?> GetInventoryAsync();
}