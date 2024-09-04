// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class Official
{
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("forum_id")]
    public int ForumId { get; set; }

    [JsonPropertyName("data")]
    public List<OfficialData> Data { get; set; } = default!;
}