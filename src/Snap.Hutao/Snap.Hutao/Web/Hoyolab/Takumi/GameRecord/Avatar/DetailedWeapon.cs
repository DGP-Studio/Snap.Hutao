// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class DetailedWeapon : Weapon
{
    [JsonPropertyName("promote_level")]
    public PromoteLevel PromoteLevel { get; set; }

    // Ignored field string name
    // Ignored field string type_name
    // Ignored field string desc
    // Ignored filed Property main_property
    // Ignored field Property sub_property
}