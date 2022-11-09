// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

/// <summary>
/// 探索派遣
/// </summary>
public class Expedition
{
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
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int RemainedTime { get; set; }

    /// <summary>
    /// 格式化的剩余时间
    /// </summary>
    public string RemainedTimeFormatted
    {
        get
        {
            if (Status == ExpeditionStatus.Finished)
            {
                return "已完成";
            }

            TimeSpan ts = new(0, 0, RemainedTime);
            return ts.Hours > 0 ? $"{ts.Hours}时" : $"{ts.Minutes}分";
        }
    }
}