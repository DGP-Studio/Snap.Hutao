// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Model.Calculable;

/// <summary>
/// 可计算的技能
/// </summary>
[HighQuality]
internal sealed class CalculableSkill
    : ObservableObject,
    ICalculableSkill,
    IMappingFrom<CalculableSkill, ProudableSkill, SkillType>,
    IMappingFrom<CalculableSkill, SkillView, SkillType>
{
    private readonly SkillType type;

    private CalculableSkill(ProudableSkill skill, SkillType type)
    {
        this.type = type;

        GroupId = skill.GroupId;
        LevelMin = 1;
        LevelMax = ProudableSkill.GetMaxLevel();
        Name = skill.Name;
        Icon = SkillIconConverter.IconNameToUri(skill.Icon);
        Quality = QualityType.QUALITY_NONE;
    }

    private CalculableSkill(SkillView skill, SkillType type)
    {
        this.type = type;

        GroupId = skill.GroupId;
        LevelMin = skill.LevelNumber;
        LevelMax = ProudableSkill.GetMaxLevel();
        Name = skill.Name;
        Icon = skill.Icon;
        Quality = QualityType.QUALITY_NONE;
    }

    /// <inheritdoc/>
    public SkillGroupId GroupId { get; }

    /// <inheritdoc/>
    public uint LevelMin { get; }

    /// <inheritdoc/>
    public uint LevelMax { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public Uri Icon { get; }

    /// <inheritdoc/>
    public QualityType Quality { get; }

    /// <inheritdoc/>
    public uint LevelCurrent
    {
        get => LocalSetting.Get(SettingKeyCurrentFromSkillType(type), LevelMin);
        set => SetProperty(LevelCurrent, value, v => LocalSetting.Set(SettingKeyCurrentFromSkillType(type), v));
    }

    /// <inheritdoc/>
    public uint LevelTarget
    {
        get => LocalSetting.Get(SettingKeyTargetFromSkillType(type), LevelMax);
        set => SetProperty(LevelTarget, value, v => LocalSetting.Set(SettingKeyTargetFromSkillType(type), v));
    }

    public static CalculableSkill From(ProudableSkill source, SkillType type)
    {
        return new(source, type);
    }

    public static CalculableSkill From(SkillView source, SkillType type)
    {
        return new(source, type);
    }

    public static string SettingKeyCurrentFromSkillType(SkillType type)
    {
        return type switch
        {
            SkillType.A => SettingKeys.CultivationAvatarSkillACurrent,
            SkillType.E => SettingKeys.CultivationAvatarSkillECurrent,
            SkillType.Q => SettingKeys.CultivationAvatarSkillQCurrent,
            _ => throw Must.NeverHappen(),
        };
    }

    public static string SettingKeyTargetFromSkillType(SkillType type)
    {
        return type switch
        {
            SkillType.A => SettingKeys.CultivationAvatarSkillATarget,
            SkillType.E => SettingKeys.CultivationAvatarSkillETarget,
            SkillType.Q => SettingKeys.CultivationAvatarSkillQTarget,
            _ => throw Must.NeverHappen(),
        };
    }
}