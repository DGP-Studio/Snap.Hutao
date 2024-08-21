// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class RecommendProperties
{
    [JsonPropertyName("sand_main_property_list")]
    public List<FightProperty> SandMainPropertyList { get; set; } = default!;

    [JsonPropertyName("goblet_main_property_list")]
    public List<FightProperty> GobletMainPropertyList { get; set; } = default!;

    [JsonPropertyName("circlet_main_property_list")]
    public List<FightProperty> CircletMainPropertyList { get; set; } = default!;

    [JsonPropertyName("sub_property_list")]
    public List<FightProperty> SubPropertyList { get; set; } = default!;
}