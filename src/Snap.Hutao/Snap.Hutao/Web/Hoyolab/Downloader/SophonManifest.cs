// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Downloader;

internal sealed class SophonManifest
{
    [JsonPropertyName("category_id")]
    public string CategoryId { get; set; } = default!;

    [JsonPropertyName("category_name")]
    public string CategoryName { get; set; } = default!;

    [JsonPropertyName("manifest")]
    public Manifest Manifest { get; set; } = default!;

    [JsonPropertyName("chunk_download")]
    public ManifestDownloadInfo ChunkDownload { get; set; } = default!;

    [JsonPropertyName("manifest_download")]
    public ManifestDownloadInfo ManifestDownload { get; set; } = default!;

    [JsonPropertyName("matching_field")]
    public string MatchingField { get; set; } = default!;

    [JsonPropertyName("stats")]
    public ManifestStats Stats { get; set; } = default!;

    [JsonPropertyName("deduplicated_stats")]
    public ManifestStats DeduplicatedStats { get; set; } = default!;
}
