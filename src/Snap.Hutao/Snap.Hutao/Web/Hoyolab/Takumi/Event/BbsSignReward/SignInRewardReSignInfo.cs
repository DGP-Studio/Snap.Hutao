// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class SignInRewardReSignInfo
{
    [JsonPropertyName("resign_cnt_daily")]
    public bool ResignCountDaily { get; set; }

    [JsonPropertyName("resign_cnt_monthly")]
    public bool ResignCountMonthly { get; set; }

    [JsonPropertyName("resign_limit_daily")]
    public bool ResignLimitDaily { get; set; }

    [JsonPropertyName("resign_limit_monthly")]
    public bool ResignLimitMonthly { get; set; }

    [JsonPropertyName("sign_cnt_missed")]
    public bool SignCountMissed { get; set; }

    [JsonPropertyName("coin_cnt")]
    public bool CoinCount { get; set; }

    [JsonPropertyName("coin_cost")]
    public bool CoinCost { get; set; }

    [JsonPropertyName("rule")]
    public string Rule { get; set; } = default!;
}