// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.Service.Hutao;

internal interface IHutaoSpiralAbyssService
{
    ValueTask<IReadOnlyList<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync(bool last = false);

    ValueTask<IReadOnlyList<AvatarCollocation>> GetAvatarCollocationsAsync(bool last = false);

    ValueTask<IReadOnlyList<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync(bool last = false);

    ValueTask<IReadOnlyList<AvatarUsageRank>> GetAvatarUsageRanksAsync(bool last = false);

    ValueTask<Overview> GetOverviewAsync(bool last = false);

    ValueTask<IReadOnlyList<TeamAppearance>> GetTeamAppearancesAsync(bool last = false);

    ValueTask<IReadOnlyList<WeaponCollocation>> GetWeaponCollocationsAsync(bool last = false);
}