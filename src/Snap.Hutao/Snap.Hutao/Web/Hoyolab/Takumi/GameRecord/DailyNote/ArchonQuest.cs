// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class ArchonQuest
{
    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ArchonQuestStatus Status { get; set; }

    /// <summary>
    /// 第X章 第Y幕
    /// </summary>
    [JsonPropertyName("chapter_num")]
    public string ChapterNum { get; set; } = default!;

    [JsonPropertyName("chapter_title")]
    public string ChapterTitle { get; set; } = default!;

    [JsonPropertyName("id")]
    public uint Id { get; set; }
}