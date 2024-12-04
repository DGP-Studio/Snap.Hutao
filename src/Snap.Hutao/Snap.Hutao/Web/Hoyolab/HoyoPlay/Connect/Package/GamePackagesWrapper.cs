// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;

internal sealed class GamePackagesWrapper
{
    [JsonPropertyName("game_packages")]
    public List<GamePackage> GamePackages { get; set; } = default!;
}
