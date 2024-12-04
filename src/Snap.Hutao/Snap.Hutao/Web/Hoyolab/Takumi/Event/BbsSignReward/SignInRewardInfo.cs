// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class SignInRewardInfo
{
    [JsonPropertyName("total_sign_day")]
    public int TotalSignDay { get; set; }

    /// <summary>
    /// yyyy-MM-dd
    /// </summary>
    [JsonPropertyName("today")]
    public string? Today { get; set; }

    [JsonPropertyName("is_sign")]
    public bool IsSign { get; set; }

    [JsonPropertyName("is_sub")]
    public bool IsSub { get; set; }

    public string Region { get; set; } = default!;

    [JsonPropertyName("sign_cnt_missed")]
    public int SignCountMissed { get; set; }

    [JsonPropertyName("short_sign_day")]
    public int ShortSignDay { get; set; }
}