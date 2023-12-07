// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 需要提供的计算信息合集
/// </summary>
[HighQuality]
internal sealed class AvatarPromotionDelta
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [JsonPropertyName("avatar_id")]
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 当前角色等级
    /// </summary>
    [JsonPropertyName("avatar_level_current")]
    public uint AvatarLevelCurrent { get; set; }

    /// <summary>
    /// 目标角色等级
    /// </summary>
    [JsonPropertyName("avatar_level_target")]
    public uint AvatarLevelTarget { get; set; }

    /// <summary>
    /// 元素类型
    /// </summary>
    [JsonPropertyName("element_attr_id")]
    public ElementAttributeId ElementAttrId { get; set; }

    /// <summary>
    /// 技能
    /// </summary>
    [JsonPropertyName("skill_list")]
    public List<PromotionDelta>? SkillList { get; set; }

    /// <summary>
    /// 武器
    /// </summary>
    [JsonPropertyName("weapon")]
    public PromotionDelta? Weapon { get; set; }

    public static AvatarPromotionDelta CreateForBaseline()
    {
        return new()
        {
            AvatarLevelTarget = LocalSetting.Get(SettingKeys.CultivationAvatarLevelTarget, 90U),
            SkillList =
            [
                new() { LevelTarget = LocalSetting.Get(SettingKeys.CultivationAvatarSkillATarget, 10U), },
                new() { LevelTarget = LocalSetting.Get(SettingKeys.CultivationAvatarSkillETarget, 10U), },
                new() { LevelTarget = LocalSetting.Get(SettingKeys.CultivationAvatarSkillQTarget, 10U), },
            ],
            Weapon = new() { LevelTarget = LocalSetting.Get(SettingKeys.CultivationWeapon90LevelTarget, 90U), },
        };
    }
}