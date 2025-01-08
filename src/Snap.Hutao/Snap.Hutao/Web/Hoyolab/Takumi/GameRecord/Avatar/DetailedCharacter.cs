// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class DetailedCharacter
{
    [JsonPropertyName("base")]
    public Character Base { get; init; } = default!;

    [JsonPropertyName("weapon")]
    public DetailedWeapon Weapon { get; init; } = default!;

    [JsonPropertyName("relics")]
    public ImmutableArray<Reliquary> Relics { get; init; }

    [JsonPropertyName("constellations")]
    public ImmutableArray<Constellation> Constellations { get; init; }

    [JsonPropertyName("costumes")]
    public List<Costume>? Costumes { get; init; }

    // HP ATK DEF EM ...
    [JsonPropertyName("selected_properties")]
    public ImmutableArray<BaseProperty> SelectedProperties { get; init; }

    [JsonPropertyName("skills")]
    public ImmutableArray<Skill> Skills { get; init; }

    [JsonPropertyName("recommend_relic_property")]
    public RecommendRelicProperty RecommendRelicProperty { get; init; } = default!;

    // Ignored field List<BaseProperty> base_properties
    // Ignored field List<BaseProperty> extra_properties
    // Ignored field List<BaseProperty> element_properties
}