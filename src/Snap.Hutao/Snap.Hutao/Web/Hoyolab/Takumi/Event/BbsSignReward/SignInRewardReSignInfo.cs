// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class SignInRewardReSignInfo
{
    [JsonPropertyName("resign_cnt_daily")]
    public uint ResignCountDaily { get; set; }

    [JsonPropertyName("resign_cnt_monthly")]
    public uint ResignCountMonthly { get; set; }

    [JsonPropertyName("resign_limit_daily")]
    public uint ResignLimitDaily { get; set; }

    [JsonPropertyName("resign_limit_monthly")]
    public uint ResignLimitMonthly { get; set; }

    [JsonPropertyName("sign_cnt_missed")]
    public uint SignCountMissed { get; set; }

    [JsonPropertyName("coin_cnt")]
    public uint CoinCount { get; set; }

    [JsonPropertyName("coin_cost")]
    public uint CoinCost { get; set; }

    [JsonPropertyName("rule")]
    public string Rule { get; set; } = default!;

    [JsonPropertyName("signed")]
    public bool Signed { get; set; }

    [JsonPropertyName("sign_days")]
    public uint SignDays { get; set; }

    [JsonPropertyName("cost")]
    public uint Cost { get; set; }

    [JsonPropertyName("month_quality_cnt")]
    public uint MonthQualityCount { get; set; }

    [JsonPropertyName("quality_cnt")]
    public uint QualityCount { get; set; }
}