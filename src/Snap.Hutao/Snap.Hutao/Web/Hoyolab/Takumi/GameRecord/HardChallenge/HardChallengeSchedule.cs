// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeSchedule
{
    [JsonPropertyName("schedule_id")]
    public required HardChallengeScheduleId ScheduleId { get; init; }

    [JsonPropertyName("start_time")]
    public required long StartTime { get; init; }

    [JsonPropertyName("end_time")]
    public required long EndTime { get; init; }

    [JsonPropertyName("start_date_time")]
    public required DateTime StartDateTime { get; init; }

    [JsonPropertyName("end_date_time")]
    public required DateTime EndDateTime { get; init; }

    [JsonPropertyName("is_valid")]
    public required bool IsValid { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}