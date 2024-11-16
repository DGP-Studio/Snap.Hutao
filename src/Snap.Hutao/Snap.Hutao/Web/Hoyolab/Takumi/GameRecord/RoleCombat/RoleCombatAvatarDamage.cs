// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatAvatarDamage
{
    [JsonPropertyName("avatar_id")]
    public AvatarId AvatarId { get; set; }

    [JsonPropertyName("avatar_icon")]
    public string AvatarIcon { get; set; } = default!;

    [JsonPropertyName("value")]
    public string Value { get; set; } = default!;

    [JsonPropertyName("rarity")]
    public QualityType Rarity { get; set; }
}