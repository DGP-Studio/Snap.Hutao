// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;
using System.Collections.Immutable;
using WebDailyNote = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Snap.Hutao.ViewModel.DailyNote;

internal sealed class DailyNoteArchonQuestView
{
    private readonly WebDailyNote? dailyNote;

    private DailyNoteArchonQuestView(WebDailyNote? dailyNote, ImmutableArray<Chapter> chapters)
    {
        this.dailyNote = dailyNote;
        Ids = chapters
            .Where(chapter => chapter.QuestType is QuestType.AQ)
            .Select(chapter => chapter.Id)
            .ToList();
    }

    public List<ChapterId> Ids { get; set; } = default!;

    public int ProgressValue
    {
        get
        {
            if (TryGetFirstArchonQuest(out ArchonQuest? quest))
            {
                return Ids.IndexOf(quest.Id);
            }

            return Ids.Count;
        }
    }

    public string ProgressFormatted
    {
        get
        {
            if (TryGetFirstArchonQuest(out ArchonQuest? quest))
            {
                return quest.Status.GetLocalizedDescription();
            }

            return SH.WebDailyNoteArchonQuestStatusFinished;
        }
    }

    public string ChapterFormatted
    {
        get
        {
            if (TryGetFirstArchonQuest(out ArchonQuest? quest))
            {
                return $"{quest.ChapterNum} {quest.ChapterTitle}";
            }

            return SH.WebDailyNoteArchonQuestChapterFinished;
        }
    }

    public static DailyNoteArchonQuestView Create(WebDailyNote? dailyNote, ImmutableArray<Chapter> chapters)
    {
        return new(dailyNote, chapters);
    }

    private bool TryGetFirstArchonQuest([NotNullWhen(true)] out ArchonQuest? archonQuest)
    {
        if (dailyNote is { ArchonQuestProgress.List: [{ } target, ..] })
        {
            archonQuest = target;
            return true;
        }

        archonQuest = default;
        return false;
    }
}