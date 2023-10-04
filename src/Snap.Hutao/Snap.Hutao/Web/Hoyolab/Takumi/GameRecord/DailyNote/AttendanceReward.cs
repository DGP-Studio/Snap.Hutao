// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class AttendanceReward
{
    [JsonPropertyName("status")]
    public AttendanceRewardStatus Status { get; set; }

    [JsonPropertyName("progress")]
    public int Progress { get; set; }
}