// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class Reliquary
{
    [JsonPropertyName("id")]
    public ReliquaryId Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    [JsonPropertyName("pos")]
    public EquipType Position { get; set; }

    [JsonPropertyName("rarity")]
    public QualityType Rarity { get; set; }

    [JsonPropertyName("level")]
    public uint Level { get; set; }

    [JsonPropertyName("set")]
    public ReliquarySet ReliquarySet { get; set; } = default!;

    [JsonPropertyName("pos_name")]
    public string PositionName { get; set; } = default!;

    [JsonPropertyName("main_property")]
    public ReliquaryProperty MainProperty { get; set; } = default!;

    [JsonPropertyName("sub_property_list")]
    public ImmutableArray<ReliquaryProperty> SubPropertyList { get; set; }
}