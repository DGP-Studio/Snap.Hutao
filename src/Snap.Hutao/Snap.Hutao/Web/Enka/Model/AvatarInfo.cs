// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Converter;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 角色信息
/// </summary>
public class AvatarInfo
{
    /// <summary>
    /// 角色Id
    /// Character ID
    /// </summary>
    [JsonPropertyName("avatarId")]
    public int AvatarId { get; set; }

    /// <summary>
    /// 基础属性
    /// Character Info Properties List
    /// <see cref="PlayerProperty.PROP_EXP"/>
    /// </summary>
    [JsonPropertyName("propMap")]
    [JsonConverter(typeof(StringEnumKeyDictionaryConverter))]
    public IDictionary<PlayerProperty, TypeValue> PropMap { get; set; } = default!;

    /// <summary>
    /// 命座 Id
    /// </summary>
    [JsonPropertyName("talentIdList")]
    public IList<int>? TalentIdList { get; set; }

    /// <summary>
    /// 属性Map
    /// Map of Character's Combat Properties.
    /// </summary>
    [JsonPropertyName("fightPropMap")]
    [JsonConverter(typeof(StringEnumKeyDictionaryConverter))]
    public IDictionary<FightProperty, double> FightPropMap { get; set; } = default!;

    /// <summary>
    /// 技能组Id
    /// Character Skill Set ID
    /// </summary>
    [JsonPropertyName("skillDepotId")]
    public int SkillDepotId { get; set; }

    /// <summary>
    /// List of Unlocked Skill Ids
    /// 被动天赋
    /// </summary>
    [JsonPropertyName("inherentProudSkillList")]
    public IList<int> InherentProudSkillList { get; set; } = default!;

    /// <summary>
    /// Map of Skill Levels
    /// </summary>
    [JsonPropertyName("skillLevelMap")]
    public IDictionary<string, int> SkillLevelMap { get; set; } = default!;

    /// <summary>
    /// 装备列表
    /// 最后一个为武器
    /// List of Equipments: Weapon, Ariftacts
    /// </summary>
    [JsonPropertyName("equipList")]
    public IList<Equip> EquipList { get; set; } = default!;

    /// <summary>
    /// 好感度信息
    /// Character Friendship Level
    /// </summary>
    [JsonPropertyName("fetterInfo")]
    public FetterInfo FetterInfo { get; set; } = default!;

    /// <summary>
    /// 皮肤 Id
    /// </summary>
    [JsonPropertyName("costumeId")]
    public int? CostumeId { get; set; }

    /// <summary>
    /// 命座额外技能等级
    /// </summary>
    [JsonPropertyName("proudSkillExtraLevelMap")]
    public IDictionary<string, int>? ProudSkillExtraLevelMap { get; set; } = default!;
}