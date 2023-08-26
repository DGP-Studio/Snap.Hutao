// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class Reward
{
    /// <summary>
    /// 月份
    /// </summary>
    [JsonPropertyName("month")]
    public int Month { get; set; }

    /// <summary>
    /// 奖励列表
    /// </summary>
    [JsonPropertyName("awards")]
    public List<Award> Awards { get; set; } = default!;

    /// <summary>
    /// 支持补签
    /// </summary>
    [JsonPropertyName("resign")]
    public bool Resign { get; set; }
}