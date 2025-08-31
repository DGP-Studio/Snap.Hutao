// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class GameReceptionData
{
    [JsonPropertyName("config")]
    public required GameReceptionDataConfiguration Config { get; init; }

    [JsonPropertyName("user_status")]
    public required JsonElement UserStatus { get; init; }

    [JsonPropertyName("exposure_times")]
    public required int ExposureTimes { get; init; }
}