// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Downloader;

internal sealed class Manifest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("checksum")]
    public string Checksum { get; set; } = default!;

    [JsonPropertyName("compressed_size")]
    public long CompressedSize { get; set; }

    [JsonPropertyName("uncompressed_size")]
    public long UncompressedSize { get; set; }
}
