// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class DailyTask
{
    [JsonPropertyName("total_num")]
    public int TotalNum { get; set; }

    [JsonPropertyName("finished_num")]
    public int FinishedNum { get; set; }

    [JsonPropertyName("is_extra_task_reward_received")]
    public bool IsExtraTaskRewardReceived { get; set; }

    [JsonPropertyName("task_rewards")]
    public List<TaskReward> TaskRewards { get; set; } = default!;

    [JsonPropertyName("attendance_rewards")]
    public List<AttendanceReward> AttendanceRewards { get; set; } = default!;

    [JsonPropertyName("attendance_visible")]
    public bool AttendanceVisible { get; set; }

    [JsonPropertyName("stored_attendance")]
    public double StoredAttendance { get; set; }

    [JsonPropertyName("stored_attendance_refresh_countdown")]
    public long StoredAttendanceRefreshCountdown { get; set; }

    [JsonIgnore]
    public string StoredAttendanceRefreshCountdownFormat
    {
        get
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(StoredAttendanceRefreshCountdown);
            return SH.FormatWebDailyNoteStoredAttendanceRefreshCountdown(timeSpan);
        }
    }
}