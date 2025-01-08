// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.RoleCombat;

namespace Snap.Hutao.Service.Hutao;

internal interface IHutaoRoleCombatService
{
    ValueTask<RoleCombatStatisticsItem> GetRoleCombatStatisticsItemAsync();
}