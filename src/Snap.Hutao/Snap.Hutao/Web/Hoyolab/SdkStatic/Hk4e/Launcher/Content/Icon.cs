// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Content;

internal sealed class Icon
{
    [JsonPropertyName("icon_id")]
    public string IconId { get; set; } = default!;

    [JsonPropertyName("img")]
    public string Image { get; set; } = default!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    [JsonPropertyName("qr_img")]
    public string QrImage { get; set; } = default!;

    [JsonPropertyName("qr_desc")]
    public string QrDescription { get; set; } = default!;

    [JsonPropertyName("img_hover")]
    public string ImageHover { get; set; } = default!;

    [JsonPropertyName("other_links")]
    public List<IconLink> OtherLinks { get; set; } = default!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("icon_link")]
    public string IconLink { get; set; } = default!;

    [JsonPropertyName("links")]
    public List<IconLink> Links { get; set; } = default!;

    [JsonPropertyName("enable_red_dot")]
    public bool EnableRedDot { get; set; }

    [JsonPropertyName("red_dot_content")]
    public string RedDotContent { get; set; } = default!;
}