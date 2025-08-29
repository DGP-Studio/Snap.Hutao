// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class LiveData
{
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("live_url")]
    public required Uri LiveUrl { get; init; }

    [JsonPropertyName("image_url")]
    public required Uri ImageUrl { get; init; }

    [JsonPropertyName("uid")]
    public required string Uid { get; init; }

    [JsonPropertyName("start_at_sec")]
    public required string StartAtSec { get; init; }

    [JsonPropertyName("end_at_sec")]
    public required string EndAtSec { get; init; }

    [JsonPropertyName("user")]
    public required LiveDataUser User { get; init; }

    [JsonPropertyName("live_end_at_sec")]
    public required string LiveEndAtSec { get; init; }

    [JsonPropertyName("living_button_text")]
    public required string LivingButtonText { get; init; }

    [JsonPropertyName("living_end_button_text")]
    public string? LivingEndButtonText { get; init; }

    [JsonPropertyName("awards")]
    public required ImmutableArray<LiveDataAward> Awards { get; init; }

    [JsonPropertyName("bg_url")]
    public required Uri BackgroundUrl { get; init; }

    [JsonPropertyName("btn_color")]
    public required string ButtonColor { get; init; }

    [JsonPropertyName("btn_text_color")]
    public required string ButtonTextColor { get; init; }

    [JsonPropertyName("award_backup_text")]
    public required string AwardBackupText { get; init; }

    [JsonPropertyName("id")]
    public required int Id { get; init; }
}