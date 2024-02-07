// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class HutaoVersionInformation
{
    [JsonPropertyName("version")]
    public Version Version { get; set; } = default!;

    [JsonPropertyName("urls")]
    public List<string> Urls { get; set; } = default!;

    [JsonPropertyName("sha256")]
    public string? Sha256 { get; set; } = default!;

    [JsonPropertyName("archive_urls")]
    public List<string> ArchiveUrls { get; set; } = default!;

    [JsonPropertyName("release_description")]
    public HutaoReleaseDescription ReleaseDescription { get; set; } = default!;
}