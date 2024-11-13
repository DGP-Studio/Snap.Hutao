// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatAvatar
{
    [JsonPropertyName("avatar_id")]
    public AvatarId AvatarId { get; set; }

    [JsonPropertyName("avatar_type")]
    public RoleCombatAvatarType AvatarType { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("element")]
    [JsonEnum(JsonEnumSerializeType.String)]
    public ElementName Element { get; set; } = default!;

    [JsonPropertyName("image")]
    public string Image { get; set; } = default!;

    [JsonPropertyName("level")]
    public Level Level { get; set; }

    [JsonPropertyName("rarity")]
    public QualityType Rarity { get; set; }
}