// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 签到结果
/// https://docs.geetest.com/sensebot/apirefer/api/server
/// </summary>
public class SignInResult
{
    /// <summary>
    /// ？？？
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = default!;

    /// <summary>
    /// 风控码 375
    /// </summary>
    [JsonPropertyName("risk_code")]
    public int RiskCode { get; set; }

    /// <summary>
    /// geetest appid
    /// </summary>
    [JsonPropertyName("gt")]
    public string Gt { get; set; } = default!;

    /// <summary>
    /// geetest challenge id
    /// </summary>
    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = default!;

    /// <summary>
    /// geetest 服务状态
    /// </summary>
    [JsonPropertyName("success")]
    public int Success { get; set; }
}