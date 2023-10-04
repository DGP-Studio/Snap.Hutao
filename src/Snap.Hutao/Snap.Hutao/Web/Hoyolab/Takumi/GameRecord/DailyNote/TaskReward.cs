// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class TaskReward
{
    [JsonPropertyName("status")]
    public TaskRewardStatus Status { get; set; }
}