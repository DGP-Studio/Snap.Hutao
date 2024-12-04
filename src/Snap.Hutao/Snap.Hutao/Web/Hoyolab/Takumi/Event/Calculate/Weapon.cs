// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal sealed class Weapon : Calculable
{
    [JsonPropertyName("weapon_cat_id")]
    public int WeaponCatId { get; set; }

    [JsonPropertyName("weapon_level")]
    public int WeaponLevel { get; set; }
}