// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssAvatar
{
    [JsonPropertyName("id")]
    public required AvatarId Id { get; init; }

    [JsonPropertyName("icon")]
    public required string Icon { get; init; }

    [JsonPropertyName("level")]
    public required int Level { get; init; }

    [JsonPropertyName("rarity")]
    public required int Rarity { get; init; }
}