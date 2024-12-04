// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class ExtraAwardInfo
{
    [JsonPropertyName("awards")]
    public List<ExtraAward> Awards { get; set; } = default!;

    [JsonPropertyName("total_cnt")]
    public int TotalCount { get; set; }

    [JsonPropertyName("ys_first_award")]
    public bool YsFirstAward { get; set; }

    [JsonPropertyName("has_short_act")]
    public bool HasShortAct { get; set; }

    [JsonPropertyName("short_act_info")]
    public ShortActInfo ShortActInfo { get; set; } = default!;
}