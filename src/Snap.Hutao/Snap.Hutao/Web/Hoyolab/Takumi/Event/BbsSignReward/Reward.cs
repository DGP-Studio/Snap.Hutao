// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 奖励
/// </summary>
public class Reward
{
    /// <summary>
    /// 月份
    /// </summary>
    [JsonPropertyName("month")]
    public string? Month { get; set; }

    /// <summary>
    /// 奖励列表
    /// </summary>
    [JsonPropertyName("awards")]
    public List<Award>? Awards { get; set; }

    /// <summary>
    /// 支持补签
    /// </summary>
    [JsonPropertyName("resign")]
    public bool Resign { get; set; }
}
