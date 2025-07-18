// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class Official
{
    [JsonPropertyName("position")]
    public required int Position { get; init; }

    [JsonPropertyName("forum_id")]
    public required int ForumId { get; init; }

    [JsonPropertyName("data")]
    public required ImmutableArray<OfficialData> Data { get; init; }
}