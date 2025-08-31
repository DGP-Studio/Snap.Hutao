// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeBest
{
    [JsonPropertyName("difficulty")]
    public required HardChallengeDifficultyLevel Difficulty { get; init; }

    [JsonPropertyName("second")]
    public required int Seconds { get; init; }

    [JsonPropertyName("icon")]
    public required string Icon { get; init; }
}