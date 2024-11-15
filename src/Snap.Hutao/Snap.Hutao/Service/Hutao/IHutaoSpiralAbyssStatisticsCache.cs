// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.Service.Hutao;

internal interface IHutaoSpiralAbyssStatisticsCache
{
    List<AvatarRankView>? AvatarUsageRanks { get; set; }

    List<AvatarRankView>? AvatarAppearanceRanks { get; set; }

    List<AvatarConstellationInfoView>? AvatarConstellationInfos { get; set; }

    List<TeamAppearanceView>? TeamAppearances { get; set; }

    Overview? Overview { get; set; }

    Dictionary<AvatarId, AvatarCollocationView>? AvatarCollocations { get; set; }

    Dictionary<WeaponId, WeaponCollocationView>? WeaponCollocations { get; set; }

    ValueTask<bool> InitializeForSpiralAbyssViewAsync();

    ValueTask<bool> InitializeForWikiAvatarViewAsync();

    ValueTask<bool> InitializeForWikiWeaponViewAsync();
}