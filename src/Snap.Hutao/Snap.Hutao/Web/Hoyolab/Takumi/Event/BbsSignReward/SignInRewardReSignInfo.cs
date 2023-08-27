// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 补签说明
/// </summary>
internal sealed class SignInRewardReSignInfo
{
    /// <summary>
    /// 当日补签次数
    /// </summary>
    [JsonPropertyName("resign_cnt_daily")]
    public bool ResignCountDaily { get; set; }

    /// <summary>
    /// 当月补签次数
    /// </summary>
    [JsonPropertyName("resign_cnt_monthly")]
    public bool ResignCountMonthly { get; set; }

    /// <summary>
    /// 当日补签次数限制
    /// </summary>
    [JsonPropertyName("resign_limit_daily")]
    public bool ResignLimitDaily { get; set; }

    /// <summary>
    /// 当月补签次数限制
    /// </summary>
    [JsonPropertyName("resign_limit_monthly")]
    public bool ResignLimitMonthly { get; set; }

    /// <summary>
    /// 漏签次数
    /// </summary>
    [JsonPropertyName("sign_cnt_missed")]
    public bool SignCountMissed { get; set; }

    /// <summary>
    /// 米游币个数
    /// </summary>
    [JsonPropertyName("coin_cnt")]
    public bool CoinCount { get; set; }

    /// <summary>
    /// 补签需要的米游币个数
    /// </summary>
    [JsonPropertyName("coin_cost")]
    public bool CoinCost { get; set; }

    /// <summary>
    /// 规则
    /// </summary>
    [JsonPropertyName("rule")]
    public string Rule { get; set; } = default!;
}