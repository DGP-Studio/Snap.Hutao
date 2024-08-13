// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Downloader;

internal sealed class ManifestDownloadInfo
{
    [JsonPropertyName("encryption")]
    public uint Encryption { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; } = default!;

    [JsonPropertyName("compression")]
    public uint Compression { get; set; }

    [JsonPropertyName("url_prefix")]
    public string UrlPrefix { get; set; } = default!;

    [JsonPropertyName("url_suffix")]
    public string UrlSuffix { get; set; } = default!;
}
