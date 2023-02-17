// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 武器
/// </summary>
[HighQuality]
internal sealed class Weapon : Calculable
{
    /// <summary>
    /// 武器Id
    /// </summary>
    [JsonPropertyName("weapon_cat_id")]
    public int WeaponCatId { get; set; }

    /// <summary>
    /// 武器品质
    /// </summary>
    [JsonPropertyName("weapon_level")]
    public int WeaponLevel { get; set; }
}