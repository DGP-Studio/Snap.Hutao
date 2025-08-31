// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

[ExtendedEnum]
internal enum AttendanceRewardStatus
{
    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusInvalid))]
    AttendanceRewardStatusInvalid,

    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusTakenAward))]
    AttendanceRewardStatusTakenAward,

    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusWaitTaken))]
    AttendanceRewardStatusWaitTaken,

    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusUnfinished))]
    AttendanceRewardStatusUnfinished,

    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusFinishedNonReward))]
    AttendanceRewardStatusFinishedNonReward,

    [LocalizationKey(nameof(SH.WebDailyNoteAttendanceRewardStatusForbid))]
    AttendanceRewardStatusForbid,
}