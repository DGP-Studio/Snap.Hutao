// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssRank
{
    [JsonPropertyName("avatar_id")]
    public required AvatarId AvatarId { get; set; }

    [JsonPropertyName("avatar_icon")]
    public required string AvatarIcon { get; set; }

    [JsonPropertyName("value")]
    public required int Value { get; set; }

    [JsonPropertyName("rarity")]
    public required int Rarity { get; set; }
}