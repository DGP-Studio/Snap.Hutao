// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.Service.Hutao;

internal interface IHutaoSpiralAbyssService
{
    ValueTask<List<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync(bool last = false);

    ValueTask<List<AvatarCollocation>> GetAvatarCollocationsAsync(bool last = false);

    ValueTask<List<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync(bool last = false);

    ValueTask<List<AvatarUsageRank>> GetAvatarUsageRanksAsync(bool last = false);

    ValueTask<Overview> GetOverviewAsync(bool last = false);

    ValueTask<List<TeamAppearance>> GetTeamAppearancesAsync(bool last = false);

    ValueTask<List<WeaponCollocation>> GetWeaponCollocationsAsync(bool last = false);
}