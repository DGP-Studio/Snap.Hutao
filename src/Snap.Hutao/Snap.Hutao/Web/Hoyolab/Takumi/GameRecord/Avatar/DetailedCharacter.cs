// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class DetailedCharacter
{
    [JsonPropertyName("base")]
    public Character Base { get; set; } = default!;

    [JsonPropertyName("weapon")]
    public DetailedWeapon Weapon { get; set; } = default!;

    [JsonPropertyName("relics")]
    public List<Reliquary> Relics { get; set; } = default!;

    [JsonPropertyName("constellations")]
    public List<Constellation> Constellations { get; set; } = default!;

    [JsonPropertyName("costumes")]
    public List<Costume>? Costumes { get; set; }

    // HP ATK DEF EM
    [JsonPropertyName("selected_properties")]
    public List<BaseProperty> SelectedProperties { get; set; } = default!;

    [JsonPropertyName("skills")]
    public List<Skill> Skills { get; set; } = default!;

    [JsonPropertyName("recommend_relic_property")]
    public RecommendRelicProperty RecommendRelicProperty { get; set; } = default!;

    // Ignored field List<BaseProperty> base_properties
    // Ignored field List<BaseProperty> extra_properties
    // Ignored field List<BaseProperty> element_properties
}