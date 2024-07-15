// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Downloader;

internal sealed class ManifestStats
{
    [JsonPropertyName("compressed_size")]
    public string CompressedSize { get; set; } = default!;

    [JsonPropertyName("uncompressed_size")]
    public string UncompressedSize { get; set; } = default!;

    [JsonPropertyName("file_count")]
    public string FileCount { get; set; } = default!;

    [JsonPropertyName("chunk_count")]
    public string ChunkCount { get; set; } = default!;
}
