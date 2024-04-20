// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class ArchonQuestProgress
{
    public List<uint> ArchonQuestIds { get; } = [
        1001U, 1002U, 1003U,
        1101U, 1102U, 1103U, 1104U,
        1201U, 1202U, 1203U, 1204U, 1205U, 1206U, 1207U,
        1301U, 1302U, 1303U, 1304U, 1305U, 1306U, 1307U, 1308U,
        1401U, 1402U, 1403U, 1404U, 1405U
    ];

    [JsonPropertyName("list")]
    public List<ArchonQuest> List { get; set; } = default!;

    public int ArchonQuestStatusValue
    {
        get
        {
            if (List.IsNullOrEmpty())
            {
                return ArchonQuestIds.Count;
            }

            return ArchonQuestIds.IndexOf(List.Single().Id);
        }
    }

    public string ArchonQuestStatusFormatted
    {
        get
        {
            if (List.IsNullOrEmpty())
            {
                return SH.WebDailyNoteArchonQuestStatusFinished;
            }

            return List.Single().Status.GetLocalizedDescription();
        }
    }

    public string ArchonQuestChapterFormatted
    {
        get
        {
            if (List.IsNullOrEmpty())
            {
                return string.Empty;
            }

            ArchonQuest quest = List.Single();
            return $"{quest.ChapterNum} {quest.ChapterTitle}";
        }
    }

    [JsonPropertyName("is_open_archon_quest")]
    public bool IsOpenArchonQuest { get; set; }

    [JsonPropertyName("is_finish_all_mainline")]
    public bool IsFinishAllMainline { get; set; }

    [JsonPropertyName("is_finish_all_interchapter")]
    public bool IsFinishAllInterchapter { get; set; }

    [JsonPropertyName("wiki_url")]
    public string WikiUrl { get; set; } = default!;
}