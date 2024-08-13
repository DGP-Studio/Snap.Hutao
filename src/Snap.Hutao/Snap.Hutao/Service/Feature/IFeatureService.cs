// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Unlocker.Island;

namespace Snap.Hutao.Service.Feature;

internal interface IFeatureService
{
    ValueTask<IslandFeature?> GetIslandFeatureAsync();
}