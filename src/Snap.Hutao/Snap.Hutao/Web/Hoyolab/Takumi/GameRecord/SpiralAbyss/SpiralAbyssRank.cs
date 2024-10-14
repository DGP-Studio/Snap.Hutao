// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssRank
{
    [JsonPropertyName("avatar_id")]
    public AvatarId AvatarId { get; set; }

    [JsonPropertyName("avatar_icon")]
    public string AvatarIcon { get; set; } = default!;

    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("rarity")]
    public int Rarity { get; set; }
}