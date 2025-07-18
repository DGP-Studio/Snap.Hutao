// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class Background
{
    [JsonPropertyName("image")]
    public required string Image { get; init; }

    [JsonPropertyName("color")]
    public required string Color { get; init; }
}