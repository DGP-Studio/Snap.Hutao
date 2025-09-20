// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Text.Json.Converter;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

internal sealed class Announcement : AnnouncementContent
{
    #region Binding

    public bool ShouldShowTimeDescription
    {
        get => Type == 1;
    }

    public string TimeDescription
    {
        get
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // 尚未开始
            if (StartTime > now)
            {
                TimeSpan span = StartTime - now;
                return span.TotalDays <= 1 ? SH.FormatWebAnnouncementTimeHoursBegin((int)span.TotalHours) : SH.FormatWebAnnouncementTimeDaysBegin((int)span.TotalDays);
            }
            else
            {
                TimeSpan span = EndTime - now;
                return span.TotalDays <= 1 ? SH.FormatWebAnnouncementTimeHoursEnd((int)span.TotalHours) : SH.FormatWebAnnouncementTimeDaysEnd((int)span.TotalDays);
            }
        }
    }

    public bool ShouldShowTimePercent
    {
        get => ShouldShowTimeDescription && TimePercent is > 0 and < 1;
    }

    public double TimePercent
    {
        get
        {
            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            TimeSpan current = currentTime - StartTime;
            TimeSpan total = EndTime - StartTime;
            return current / total;
        }
    }

    public string TimeFormatted
    {
        get => $"{StartTime.ToLocalTime():yyyy.MM.dd HH:mm} - {EndTime.ToLocalTime():yyyy.MM.dd HH:mm}";
    }
    #endregion

    [JsonPropertyName("type_label")]
    public string TypeLabel { get; set; } = default!;

    [JsonPropertyName("tag_label")]
    public string TagLabel { get; set; } = default!;

    [JsonPropertyName("tag_icon")]
    public string TagIcon { get; set; } = default!;

    [JsonPropertyName("login_alert")]
    public int LoginAlert { get; set; }

    [JsonPropertyName("start_time")]
    [JsonConverter(typeof(SimpleDateTimeOffsetConverter))]
    public DateTimeOffset StartTime { get; set; }

    [JsonPropertyName("end_time")]
    [JsonConverter(typeof(SimpleDateTimeOffsetConverter))]
    public DateTimeOffset EndTime { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("remind")]
    public int Remind { get; set; }

    [JsonPropertyName("alert")]
    public int Alert { get; set; }

    [JsonPropertyName("tag_start_time")]
    public string TagStartTime { get; set; } = default!;

    [JsonPropertyName("tag_end_time")]
    public string TagEndTime { get; set; } = default!;

    [JsonPropertyName("remind_ver")]
    public int RemindVersion { get; set; }

    [JsonPropertyName("has_content")]
    public bool HasContent { get; set; }
}