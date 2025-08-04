// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Yae.Achievement;

internal sealed class NativeConfiguration
{
    [JsonPropertyName("storeCmdId")]
    public required uint StoreCmdId { get; init; }

    [JsonPropertyName("achievementCmdId")]
    public required uint AchievementCmdId { get; init; }

    [JsonPropertyName("methodRva")]
    public required MethodRvaWrapper MethodRva { get; init; }
}