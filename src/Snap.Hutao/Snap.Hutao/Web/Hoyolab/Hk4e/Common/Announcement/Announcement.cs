// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1124")]
internal sealed class Announcement : AnnouncementContent
{
    #region Binding

    /// <summary>
    /// 是否应展示时间
    /// </summary>
    public bool ShouldShowTimeDescription
    {
        get => Type == 1;
    }

    /// <summary>
    /// 时间
    /// </summary>
    public string TimeDescription
    {
        get
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // 尚未开始
            if (StartTime > now)
            {
                TimeSpan span = StartTime - now;
                if (span.TotalDays <= 1)
                {
                    return SH.WebAnnouncementTimeHoursBeginFormat.Format((int)span.TotalHours);
                }

                return SH.WebAnnouncementTimeDaysBeginFormat.Format((int)span.TotalDays);
            }
            else
            {
                TimeSpan span = EndTime - now;
                if (span.TotalDays <= 1)
                {
                    return SH.WebAnnouncementTimeHoursEndFormat.Format((int)span.TotalHours);
                }

                return SH.WebAnnouncementTimeDaysEndFormat.Format((int)span.TotalDays);
            }
        }
    }

    /// <summary>
    /// 是否应显示时间百分比
    /// </summary>
    public bool ShouldShowTimePercent
    {
        get => ShouldShowTimeDescription && (TimePercent > 0 && TimePercent < 1);
    }

    /// <summary>
    /// 时间百分比
    /// </summary>
    public double TimePercent
    {
        get
        {
            // TODO: validate correctness
            // UTC+8
            DateTimeOffset currentTime = DateTimeOffset.UtcNow.AddHours(8);
            TimeSpan current = currentTime - StartTime;
            TimeSpan total = EndTime - StartTime;
            return current / total;
        }
    }

    /// <summary>
    /// 格式化的起止时间
    /// </summary>
    public string TimeFormatted
    {
        get => $"{StartTime:yyyy.MM.dd HH:mm} - {EndTime:yyyy.MM.dd HH:mm}";
    }
    #endregion

    /// <summary>
    /// 类型标签
    /// </summary>
    [JsonPropertyName("type_label")]
    public string TypeLabel { get; set; } = default!;

    /// <summary>
    /// 标签文本
    /// </summary>
    [JsonPropertyName("tag_label")]
    public string TagLabel { get; set; } = default!;

    /// <summary>
    /// 标签图标
    /// </summary>
    [JsonPropertyName("tag_icon")]
    public string TagIcon { get; set; } = default!;

    /// <summary>
    /// 登录提醒
    /// </summary>
    [JsonPropertyName("login_alert")]
    public int LoginAlert { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [JsonPropertyName("start_time")]
    [JsonConverter(typeof(Core.Json.Converter.DateTimeOffsetConverter))]
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [JsonPropertyName("end_time")]
    [JsonConverter(typeof(Core.Json.Converter.DateTimeOffsetConverter))]
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// 提醒
    /// </summary>
    [JsonPropertyName("remind")]
    public int Remind { get; set; }

    /// <summary>
    /// 通知
    /// </summary>
    [JsonPropertyName("alert")]
    public int Alert { get; set; }

    /// <summary>
    /// 标签开始时间
    /// </summary>
    [JsonPropertyName("tag_start_time")]
    public string TagStartTime { get; set; } = default!;

    /// <summary>
    /// 标签结束时间
    /// </summary>
    [JsonPropertyName("tag_end_time")]
    public string TagEndTime { get; set; } = default!;

    /// <summary>
    /// 提醒版本
    /// </summary>
    [JsonPropertyName("remind_ver")]
    public int RemindVersion { get; set; }

    /// <summary>
    /// 是否含有内容
    /// </summary>
    [JsonPropertyName("has_content")]
    public bool HasContent { get; set; }
}