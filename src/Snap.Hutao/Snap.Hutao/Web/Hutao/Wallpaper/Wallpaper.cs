// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Wallpaper;

internal sealed class Wallpaper
{
    [JsonPropertyName("url")]
    public required Uri Url { get; init; }

    [JsonPropertyName("source_url")]
    public required Uri SourceUrl { get; init; }

    [JsonPropertyName("author")]
    public required string Author { get; init; }

    [JsonPropertyName("uploader")]
    public required string Uploader { get; init; }
}