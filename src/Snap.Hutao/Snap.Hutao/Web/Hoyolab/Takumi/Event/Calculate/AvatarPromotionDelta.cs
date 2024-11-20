// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal sealed class AvatarPromotionDelta
{
    [JsonPropertyName("avatar_id")]
    public AvatarId AvatarId { get; set; }

    [JsonPropertyName("avatar_level_current")]
    public uint AvatarLevelCurrent { get; set; }

    [JsonPropertyName("avatar_level_target")]
    public uint AvatarLevelTarget { get; set; }

    [JsonPropertyName("element_attr_id")]
    public ElementAttributeId ElementAttrId { get; set; }

    [JsonPropertyName("skill_list")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ImmutableArray<PromotionDelta> SkillList { get; set; }

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