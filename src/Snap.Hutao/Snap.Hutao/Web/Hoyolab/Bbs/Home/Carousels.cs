// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class Carousels
{
    [JsonPropertyName("position")]
    public required int Position { get; init; }

    [JsonPropertyName("data")]
    public required ImmutableArray<HomeBanner> Data { get; init; }
}