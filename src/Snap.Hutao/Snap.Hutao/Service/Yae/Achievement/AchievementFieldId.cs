// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Yae.Achievement;

internal sealed class AchievementFieldId
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("status")]
    public int Status { get; init; }

    [JsonPropertyName("cur_progress")]
    public int CurrentProgress { get; init; }

    [JsonPropertyName("total_progress")]
    public int TotalProgress { get; init; }

    [JsonPropertyName("finish_timestamp")]
    public int FinishTimestamp { get; init; }
}