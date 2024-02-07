// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Content;

internal sealed class GameContent
{
    [JsonPropertyName("adv")]
    public Advertisement? Advertisement { get; set; } = default!;

    [JsonPropertyName("banner")]
    public List<Banner> Banners { get; set; } = default!;

    [JsonPropertyName("icon")]
    public List<Icon> Icons { get; set; } = default!;

    [JsonPropertyName("post")]
    public List<Post> Posts { get; set; } = default!;

    [JsonPropertyName("qq")]
    public List<QQ> QQs { get; set; } = default!;

    [JsonPropertyName("more")]
    public More More { get; set; } = default!;

    [JsonPropertyName("links")]
    public Link Links { get; set; } = default!;
}