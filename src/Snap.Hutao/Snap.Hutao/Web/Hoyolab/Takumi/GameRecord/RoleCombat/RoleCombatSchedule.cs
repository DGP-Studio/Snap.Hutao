// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatSchedule
{
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; } = default!;

    [JsonPropertyName("end_time")]
    public long EndTime { get; set; } = default!;

    [JsonPropertyName("schedule_type")]
    public ScheduleType ScheduleType { get; set; } = default!;

    [JsonPropertyName("schedule_id")]
    public int ScheduleId { get; set; }

    [JsonPropertyName("start_date_time")]
    public DateTime StartDateTime { get; set; } = default!;

    [JsonPropertyName("end_date_time")]
    public DateTime EndDateTime { get; set; } = default!;
}