// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

internal sealed class VersionItem
{
    [JsonPropertyName("remoteName")]
    public string RelativePath { get; set; } = default!;

    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = default!;

    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }
}