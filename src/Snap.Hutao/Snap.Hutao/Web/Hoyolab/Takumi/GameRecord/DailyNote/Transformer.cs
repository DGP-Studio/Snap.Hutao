// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

internal sealed class Transformer
{
    [JsonPropertyName("obtained")]
    [MemberNotNullWhen(true, nameof(RecoveryTime))]
    public bool Obtained { get; set; }

    [JsonPropertyName("recovery_time")]
    public RecoveryTime? RecoveryTime { get; set; }

    public string ObtainedAndReachedFormatted
    {
        get => Obtained ? RecoveryTime.ReachedFormatted : SH.WebDailyNoteTransformerNotObtained;
    }

    public string ObtainedAndTimeFormatted
    {
        get => Obtained ? RecoveryTime.TimeFormatted : SH.WebDailyNoteTransformerNotObtainedDetail;
    }

    [JsonPropertyName("wiki")]
    public Uri Wiki { get; set; } = default!;

    [JsonPropertyName("noticed")]
    public bool Noticed { get; set; }

    [JsonPropertyName("latest_job_id")]
    public string LastJobId { get; set; } = default!;
}
