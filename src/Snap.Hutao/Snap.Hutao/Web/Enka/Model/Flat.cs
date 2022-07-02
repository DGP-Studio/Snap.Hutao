// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 平展值
/// </summary>
public class Flat
{
    /// <summary>
    /// 名称
    /// Hash for Equipment Name
    /// </summary>
    [JsonPropertyName("nameTextMapHash")]
    public string NameTextMapHash { get; set; } = default!;

    /// <summary>
    /// 套装名称
    /// Hash for Artifact Set Name
    /// </summary>
    [JsonPropertyName("setNameTextMapHash")]
    public string? SetNameTextMapHash { get; set; }

    /// <summary>
    /// 等级
    /// Rarity Level of Equipment
    /// </summary>
    [JsonPropertyName("rankLevel")]
    public int RankLevel { get; set; }

    /// <summary>
    /// 圣遗物主属性
    /// Artifact Main Stat
    /// </summary>
    [JsonPropertyName("reliquaryMainstat")]
    public ReliquaryMainstat? ReliquaryMainstat { get; set; }

    /// <summary>
    /// 圣遗物副属性
    /// List of Artifact Substats
    /// </summary>
    [JsonPropertyName("reliquarySubstats")]
    public IList<ReliquarySubstat>? ReliquarySubstats { get; set; }

    /// <summary>
    /// 物品类型
    /// Equipment Type: Weapon or Artifact
    /// ITEM_WEAPON
    /// ITEM_RELIQUARY
    /// </summary>
    [JsonPropertyName("itemType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ItemType ItemType { get; set; } = default!;

    /// <summary>
    /// 图标
    /// Equipment Icon Name
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 圣遗物类型
    /// 当为武器时
    /// 值为 <see cref="EquipType.EQUIP_NONE"/>
    /// </summary>
    [JsonPropertyName("equipType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EquipType EquipType { get; set; }

    /// <summary>
    /// 武器主副属性
    /// 0 基础攻击力
    /// 1 主属性
    /// List of Weapon Stat: Base ATK, Substat
    /// </summary>
    [JsonPropertyName("weaponStats")]
    public IList<WeaponStat>? WeaponStats { get; set; }
}
