// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class RecoveryTime
{
    [JsonPropertyName("Day")]
    public int Day { get; set; }

    [JsonPropertyName("Hour")]
    public int Hour { get; set; }

    [JsonPropertyName("Minute")]
    public int Minute { get; set; }

    [JsonPropertyName("Second")]
    public int Second { get; set; }

    [JsonPropertyName("reached")]
    public bool Reached { get; set; }

    [JsonIgnore]
    public int TotalSeconds
    {
        get => (60 * 60 * 24 * 7) - (Second + (60 * Minute) + (60 * 60 * Hour) + (60 * 60 * 24 * Day));
    }

    [JsonIgnore]
    public string TimeFormatted
    {
        get => Reached
            ? SH.WebDailyNoteTransformerReady
            : new StringBuilder()
                .AppendIf(Day > 0, SH.FormatWebDailyNoteTransformerDays(Day))
                .AppendIf(Hour > 0, SH.FormatWebDailyNoteTransformerHours(Hour))
                .AppendIf(Minute > 0, SH.FormatWebDailyNoteTransformerMinutes(Minute))
                .AppendIf(Second > 0, SH.FormatWebDailyNoteTransformerSeconds(Second))
                .Append(SH.WebDailyNoteTransformerAppend)
                .ToString();
    }

    [JsonIgnore]
    public string TimeLeftFormatted
    {
        get => Reached
            ? SH.WebDailyNoteTransformerReady
            : new StringBuilder()
                .AppendIf(Day > 0, SH.FormatWebDailyNoteTransformerDays(Day))
                .AppendIf(Hour > 0, SH.FormatWebDailyNoteTransformerHours(Hour))
                .AppendIf(Minute > 0, SH.FormatWebDailyNoteTransformerMinutes(Minute))
                .AppendIf(Second > 0, SH.FormatWebDailyNoteTransformerSeconds(Second))
                .ToString();
    }

    [JsonIgnore]
    public string ReachedFormatted
    {
        get => Reached ? SH.WebDailyNoteTransformerReached : SH.WebDailyNoteTransformerNotReached;
    }
}