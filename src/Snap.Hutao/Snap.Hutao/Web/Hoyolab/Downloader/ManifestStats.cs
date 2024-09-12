// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Downloader;

internal sealed class ManifestStats
{
    [JsonPropertyName("compressed_size")]
    public long CompressedSize { get; set; } = default!;

    [JsonPropertyName("uncompressed_size")]
    public long UncompressedSize { get; set; } = default!;

    [JsonPropertyName("file_count")]
    public uint FileCount { get; set; } = default!;

    [JsonPropertyName("chunk_count")]
    public uint ChunkCount { get; set; } = default!;
}
