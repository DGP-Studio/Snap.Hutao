// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatSchedule
{
    [JsonPropertyName("start_time")]
    public required long StartTime { get; init; }

    [JsonPropertyName("end_time")]
    public required long EndTime { get; init; }

    [JsonPropertyName("schedule_type")]
    public required ScheduleType ScheduleType { get; init; }

    [JsonPropertyName("schedule_id")]
    public required RoleCombatScheduleId ScheduleId { get; init; }

    [JsonPropertyName("start_date_time")]
    public required DateTime StartDateTime { get; init; }

    [JsonPropertyName("end_date_time")]
    public required DateTime EndDateTime { get; init; }
}