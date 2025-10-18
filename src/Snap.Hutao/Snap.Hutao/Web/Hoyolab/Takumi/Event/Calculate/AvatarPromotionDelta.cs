// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

[BindableCustomPropertyProvider]
internal sealed partial class AvatarPromotionDelta : ObservableObject
{
    [ObservableProperty]
    [JsonPropertyName("avatar_id")]
    public partial AvatarId AvatarId { get; set; }

    [ObservableProperty]
    [JsonPropertyName("avatar_level_current")]
    public partial uint AvatarLevelCurrent { get; set; }

    [ObservableProperty]
    [JsonPropertyName("avatar_level_target")]
    public partial uint AvatarLevelTarget { get; set; }

    [ObservableProperty]
    [JsonPropertyName("element_attr_id")]
    public partial ElementAttributeId ElementAttrId { get; set; }

    [ObservableProperty]
    [JsonPropertyName("skill_list")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public partial ImmutableArray<PromotionDelta> SkillList { get; set; }

    [ObservableProperty]
    [JsonPropertyName("weapon")]
    public partial PromotionDelta? Weapon { get; set; }

    [ObservableProperty]
    [JsonPropertyName("from_user_sync")]
    public partial bool FromUserSync { get; set; } = true;

    [ObservableProperty]
    [JsonPropertyName("avatar_promote_level")]
    public partial uint AvatarPromoteLevel { get; set; }

    internal IProperty<bool> IsViewUnloaded { get; } = Property.Create(false);

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

    public static AvatarPromotionDelta CreateForAvatarMaxConsumption(Model.Metadata.Avatar.Avatar avatar)
    {
        return new()
        {
            AvatarId = avatar.Id,
            AvatarLevelCurrent = 1,
            AvatarLevelTarget = 90,
            SkillList = avatar.SkillDepot.CompositeSkillsNoInherents.SelectAsArray(static skill => new PromotionDelta
            {
                Id = skill.GroupId,
                LevelCurrent = 1,
                LevelTarget = 10,
            }),
        };
    }
}