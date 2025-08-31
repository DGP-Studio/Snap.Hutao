// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

[ExtendedEnum]
internal enum ArchonQuestStatus
{
    [LocalizationKey(nameof(SH.WebDailyNoteArchonQuestStatusFinished))]
    StatusFinished,

    [LocalizationKey(nameof(SH.WebDailyNoteArchonQuestStatusOngoing))]
    StatusOngoing,

    [LocalizationKey(nameof(SH.WebDailyNoteArchonQuestStatusNotOpen))]
    StatusNotOpen,
}