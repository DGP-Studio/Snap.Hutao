// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Island;
using Snap.Hutao.Service.Yae.Achievement;

namespace Snap.Hutao.Service.Feature;

internal interface IFeatureService
{
    ValueTask<IslandFeature2?> GetIslandFeatureAsync(string tag);

    ValueTask<AchievementFieldId?> GetAchievementFieldIdAsync(string tag);
}