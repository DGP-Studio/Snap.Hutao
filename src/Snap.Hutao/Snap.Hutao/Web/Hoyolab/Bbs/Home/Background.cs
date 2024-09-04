// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class Background
{
    [JsonPropertyName("image")]
    public string Image { get; set; } = default!;

    [JsonPropertyName("color")]
    public string Color { get; set; } = default!;
}