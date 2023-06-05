// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 角色
/// </summary>
[HighQuality]
internal sealed class Avatar : Calculable
{
    /// <summary>
    /// 角色的星级
    /// </summary>
    [JsonPropertyName("avatar_level")]
    public QualityType AvatarLevel { get; set; }

    /// <summary>
    /// 武器类型
    /// </summary>
    [JsonPropertyName("weapon_cat_id")]
    public WeaponType WeaponCatId { get; set; }

    /// <summary>
    /// 元素类型
    /// </summary>
    [JsonPropertyName("element_attr_id")]
    public ElementAttributeId ElementAttrId { get; set; }
}