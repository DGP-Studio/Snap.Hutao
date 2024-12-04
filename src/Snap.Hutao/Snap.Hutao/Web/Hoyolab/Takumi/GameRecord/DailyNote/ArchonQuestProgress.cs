// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class ArchonQuestProgress
{
    [JsonPropertyName("list")]
    public List<ArchonQuest> List { get; set; } = default!;

    [JsonPropertyName("is_open_archon_quest")]
    public bool IsOpenArchonQuest { get; set; }

    [JsonPropertyName("is_finish_all_mainline")]
    public bool IsFinishAllMainline { get; set; }

    [JsonPropertyName("is_finish_all_interchapter")]
    public bool IsFinishAllInterchapter { get; set; }

    [JsonPropertyName("wiki_url")]
    public string WikiUrl { get; set; } = default!;
}