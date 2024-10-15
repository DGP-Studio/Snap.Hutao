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

    public List<ChapterId> Ids { get; set; }

    public int ProgressValue
    {
        get => TryGetFirstArchonQuest(out ArchonQuest? quest) ? Ids.IndexOf(quest.Id) : Ids.Count;
    }

    public string ProgressFormatted
    {
        get => TryGetFirstArchonQuest(out ArchonQuest? quest) ? quest.Status.GetLocalizedDescription() : SH.WebDailyNoteArchonQuestStatusFinished;
    }

    public string ChapterFormatted
    {
        get => TryGetFirstArchonQuest(out ArchonQuest? quest) ? $"{quest.ChapterNum} {quest.ChapterTitle}" : SH.WebDailyNoteArchonQuestChapterFinished;
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