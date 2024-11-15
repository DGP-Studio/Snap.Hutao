// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

internal sealed class Flat
{
    [JsonPropertyName("nameTextMapHash")]
    public ulong NameTextMapHash { get; set; }

    [JsonPropertyName("setNameTextMapHash")]
    public ulong SetNameTextMapHash { get; set; }

    [JsonPropertyName("rankLevel")]
    public QualityType RankLevel { get; set; }

    [JsonPropertyName("reliquaryMainstat")]
    public ReliquaryMainstat? ReliquaryMainstat { get; set; }

    [JsonPropertyName("reliquarySubstats")]
    public List<ReliquarySubstat>? ReliquarySubstats { get; set; }

    [JsonPropertyName("itemType")]
    [JsonEnum(JsonEnumSerializeType.String)]
    public ItemType ItemType { get; set; } = default!;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    [JsonPropertyName("equipType")]
    [JsonEnum(JsonEnumSerializeType.String)]
    public EquipType EquipType { get; set; }

    [JsonPropertyName("weaponStats")]
    public List<WeaponStat>? WeaponStats { get; set; }
}