// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class ShortExtraAward
{
    [JsonPropertyName("has_extra_award")]
    public bool HasExtraAward { get; set; }

    [JsonPropertyName("start_time")]
    public string StartTime { get; set; } = default!;

    [JsonPropertyName("end_time")]
    public string EndTime { get; set; } = default!;

    [JsonPropertyName("list")]
    public List<JsonElement> List { get; set; } = default!;

    [JsonPropertyName("start_timestamp")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long StartTimestamp { get; set; } = default!;

    [JsonPropertyName("end_timestamp")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long EndTimestamp { get; set; } = default!;
}