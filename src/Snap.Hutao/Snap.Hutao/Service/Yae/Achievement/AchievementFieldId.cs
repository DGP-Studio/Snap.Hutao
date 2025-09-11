// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Yae.Achievement;

internal sealed class AchievementFieldId
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("status")]
    public required int Status { get; init; }

    [JsonPropertyName("currentProgress")]
    public required int CurrentProgress { get; init; }

    [JsonPropertyName("totalProgress")]
    public required int TotalProgress { get; init; }

    [JsonPropertyName("finishTimestamp")]
    public required int FinishTimestamp { get; init; }

    [JsonPropertyName("nativeConfig")]
    public required NativeConfiguration NativeConfig { get; init; }
}