// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;

internal class PackageSegment
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = default!;

    [JsonPropertyName("size")]
    public string Size { get; set; } = default!;

    [JsonPropertyName("decompressed_size")]
    public string DecompressedSize { get; set; } = default!;
}
