// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 角色信息
/// </summary>
[HighQuality]
internal sealed class AvatarInfo
{
    /// <summary>
    /// 角色Id
    /// Character ID
    /// </summary>
    [JsonPropertyName("avatarId")]
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 基础属性
    /// Character Info Properties List
    /// <see cref="PlayerProperty.PROP_EXP"/>
    /// </summary>
    [MaybeNull]
    [JsonPropertyName("propMap")]
    public Dictionary<PlayerProperty, TypeValue> PropMap { get; set; } = default!;

    /// <summary>
    /// 命座 Id
    /// </summary>
    [JsonPropertyName("talentIdList")]
    public List<SkillId>? TalentIdList { get; set; }

    /// <summary>
    /// 属性 Map
    /// Map of Character's Combat Properties.
    /// </summary>
    [JsonPropertyName("fightPropMap")]
    public Dictionary<FightProperty, float> FightPropMap { get; set; } = default!;

    /// <summary>
    /// 技能组Id
    /// Character Skill Set ID
    /// </summary>
    [JsonPropertyName("skillDepotId")]
    public uint SkillDepotId { get; set; }

    /// <summary>
    /// List of Unlocked Skill Ids
    /// 被动天赋
    /// </summary>
    [JsonPropertyName("inherentProudSkillList")]
    public List<SkillId> InherentProudSkillList { get; set; } = default!;

    /// <summary>
    /// Map of Skill Levels
    /// </summary>
    [JsonPropertyName("skillLevelMap")]
    public Dictionary<SkillId, SkillLevel> SkillLevelMap { get; set; } = default!;

    /// <summary>
    /// 装备列表
    /// 最后一个为武器
    /// List of Equipments: Weapon, Artifacts
    /// </summary>
    [MaybeNull]
    [JsonPropertyName("equipList")]
    public List<Equip> EquipList { get; set; } = default!;

    /// <summary>
    /// 好感度信息
    /// Character Friendship Level
    /// </summary>
    [MaybeNull]
    [JsonPropertyName("fetterInfo")]
    public FetterInfo FetterInfo { get; set; } = default!;

    /// <summary>
    /// 皮肤 Id
    /// </summary>
    [JsonPropertyName("costumeId")]
    public CostumeId? CostumeId { get; set; }

    /// <summary>
    /// 命座额外技能等级
    /// </summary>
    [JsonPropertyName("proudSkillExtraLevelMap")]
    public Dictionary<SkillGroupId, SkillLevel>? ProudSkillExtraLevelMap { get; set; } = default!;
}