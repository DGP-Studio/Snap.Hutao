// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssAvatar
{
    [JsonPropertyName("id")]
    public AvatarId Id { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("rarity")]
    public int Rarity { get; set; }
}