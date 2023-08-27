// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 签到信息
/// </summary>
internal sealed class SignInRewardInfo
{
    /// <summary>
    /// 累积签到天数
    /// </summary>
    [JsonPropertyName("total_sign_day")]
    public int TotalSignDay { get; set; }

    /// <summary>
    /// yyyy-MM-dd
    /// </summary>
    [JsonPropertyName("today")]
    public string? Today { get; set; }

    /// <summary>
    /// 今日是否已签到
    /// </summary>
    [JsonPropertyName("is_sign")]
    public bool IsSign { get; set; }

    /// <summary>
    /// ？
    /// </summary>
    [JsonPropertyName("is_sub")]
    public bool IsSub { get; set; }

    /// <summary>
    /// 是否首次绑定
    /// </summary>
    [JsonPropertyName("first_bind")]
    public bool FirstBind { get; set; }

    /// <summary>
    /// 是否为当月第一次
    /// </summary>
    [JsonPropertyName("month_first")]
    public bool MonthFirst { get; set; }

    /// <summary>
    /// 漏签天数
    /// </summary>
    [JsonPropertyName("sign_cnt_missed")]
    public bool SignCountMissed { get; set; }
}