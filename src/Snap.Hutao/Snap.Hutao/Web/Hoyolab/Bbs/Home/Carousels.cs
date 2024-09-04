// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class Carousels
{
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("data")]
    public List<HomeBanner> Data { get; set; } = default!;
}