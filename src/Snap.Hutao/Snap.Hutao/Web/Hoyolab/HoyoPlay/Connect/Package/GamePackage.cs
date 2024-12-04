// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;

internal sealed class GamePackage : GameIndexedObject
{
    [JsonPropertyName("main")]
    public GameBranch Main { get; set; } = default!;

    [JsonPropertyName("pre_download")]
    public GameBranch PreDownload { get; set; } = default!;
}
