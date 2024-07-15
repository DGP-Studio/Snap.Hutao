// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Downloader;

internal sealed class Manifest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("checksum")]
    public string Checksum { get; set; } = default!;

    [JsonPropertyName("compressed_size")]
    public string CompressedSize { get; set; } = default!;

    [JsonPropertyName("uncompressed_size")]
    public string UncompressedSize { get; set; } = default!;
}
