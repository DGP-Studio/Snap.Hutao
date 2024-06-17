// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;

internal sealed class GamePackage
{
    [JsonPropertyName("game")]
    public GameIdentifier Game { get; set; } = default!;

    [JsonPropertyName("main")]
    public Game Main { get; set; } = default!;

    [JsonPropertyName("pre_download")]
    public Game PreDownload { get; set; } = default!;
}
