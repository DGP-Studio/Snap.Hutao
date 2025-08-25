// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;
using System.Globalization;
using WebDailyNote = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Snap.Hutao.ViewModel.DailyNote;

internal sealed class DailyNoteArchonQuestView
{
    private DailyNoteArchonQuestView(WebDailyNote? dailyNote, ImmutableArray<Chapter> chapters)
    {
        Ids = [.. chapters.Where(chapter => chapter.QuestType is QuestType.AQ).Select(chapter => chapter.Id)];

        if (dailyNote is { ArchonQuestProgress.List: [{ } quest, ..] })
        {
            ProgressValue = Ids.IndexOf(quest.Id);
            FormattedProgress = quest.Status.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture);
            FormattedChapter = $"{quest.ChapterNum} {quest.ChapterTitle}";
        }
        else
        {
            ProgressValue = Ids.Length;
            FormattedProgress = SH.WebDailyNoteArchonQuestStatusFinished;
            FormattedChapter = SH.WebDailyNoteArchonQuestChapterFinished;
        }
    }

    public ImmutableArray<ChapterId> Ids { get; }

    public int ProgressValue { get; }

    public string? FormattedProgress { get; }

    public string FormattedChapter { get; }

    public static DailyNoteArchonQuestView Create(WebDailyNote? dailyNote, ImmutableArray<Chapter> chapters)
    {
        return new(dailyNote, chapters);
    }
}