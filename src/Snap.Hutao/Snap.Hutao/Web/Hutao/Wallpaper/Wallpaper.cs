// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Wallpaper;

internal sealed class Wallpaper
{
    [JsonPropertyName("url")]
    public Uri Url { get; set; } = default!;

    [JsonPropertyName("source_url")]
    public string SourceUrl { get; set; } = default!;

    [JsonPropertyName("author")]
    public string Author { get; set; } = default!;

    [JsonPropertyName("uploader")]
    public string Uploader { get; set; } = default!;
}