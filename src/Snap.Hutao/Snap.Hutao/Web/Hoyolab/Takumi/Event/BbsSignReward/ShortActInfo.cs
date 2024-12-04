// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class ShortActInfo
{
    [JsonPropertyName("awards")]
    public List<JsonElement> Awards { get; set; } = default!;

    [JsonPropertyName("start_timestamp")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long StartTimestamp { get; set; }

    [JsonPropertyName("end_timestamp")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long EndTimestamp { get; set; }

    [JsonPropertyName("total_cnt")]
    public int TotalCount { get; set; }
}