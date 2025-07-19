// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class Discussion
{
    [JsonPropertyName("icon")]
    public required string Icon { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }
}