// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 探索派遣
/// </summary>
[HighQuality]
internal sealed class Expedition
{
    private const string Bennett = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Bennett.png";
    private const string Chongyun = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Chongyun.png";
    private const string Fischl = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Fischl.png";
    private const string Keqing = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Keqing.png";
    private const string Sara = "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_Sara.png";

    private static readonly ImmutableList<string> ShortExpeditionTimeAvatars = new List<string>()
    {
        Bennett, Chongyun, Fischl, Keqing, Sara,
    }.ToImmutableList();

    /// <summary>
    /// 图标
    /// </summary>
    [JsonPropertyName("avatar_side_icon")]
    public Uri AvatarSideIcon { get; set; } = default!;

    /// <summary>
    /// 状态 Ongoing:派遣中 Finished:已完成
    /// </summary>
    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ExpeditionStatus Status { get; set; }

    /// <summary>
    /// 剩余时间
    /// </summary>
    [JsonPropertyName("remained_time")]
    public int RemainedTime { get; set; }

    /// <summary>
    /// 已过时间
    /// </summary>
    [JsonIgnore]
    public int PassedTime
    {
        get
        {
            // 当派遣非20小时的任务时无法兼容
            int hours = ShortExpeditionTimeAvatars.Contains(AvatarSideIcon.ToString()) ? 15 : 20;
            return (hours * 60 * 60) - RemainedTime;
        }
    }

    /// <summary>
    /// 总时间
    /// </summary>
    [JsonIgnore]
    public int TotalTime
    {
        get
        {
            // 当派遣非20小时的任务时无法兼容
            int hours = ShortExpeditionTimeAvatars.Contains(AvatarSideIcon.ToString()) ? 15 : 20;
            return hours * 60 * 60;
        }
    }

    /// <summary>
    /// 格式化的剩余时间
    /// </summary>
    [JsonIgnore]
    public string RemainedTimeFormatted
    {
        get
        {
            if (Status == ExpeditionStatus.Finished)
            {
                return SH.ServiceDailyNoteNotifierExpeditionAdaptiveHint;
            }

            TimeSpan ts = new(0, 0, RemainedTime);
            return ts.Hours > 0
                ? string.Format(SH.WebDailyNoteExpeditionRemainHoursFormat, ts.Hours)
                : string.Format(SH.WebDailyNoteExpeditionRemainMinutesFormat, ts.Minutes);
        }
    }
}