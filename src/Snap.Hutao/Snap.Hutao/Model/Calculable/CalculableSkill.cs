// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Model.Calculable;

internal sealed partial class CalculableSkill : ObservableObject, ICalculableSkill
{
    // Only persists current level for non-view skills
    private readonly bool persistsCurrentLevel;
    private readonly SkillType type;

    private CalculableSkill(ProudSkill skill, SkillType type)
    {
        persistsCurrentLevel = true;
        this.type = type;

        GroupId = skill.GroupId;
        LevelMin = 1;
        LevelMax = ProudSkill.GetMaxLevel();
        Name = skill.Name;
        Icon = SkillIconConverter.IconNameToUri(skill.Icon);
        Quality = QualityType.QUALITY_NONE;
    }

    private CalculableSkill(SkillView skill, SkillType type)
    {
        persistsCurrentLevel = false;
        this.type = type;

        GroupId = skill.GroupId;
        LevelMin = skill.LevelNumber;
        LevelMax = ProudSkill.GetMaxLevel();
        Name = skill.Name;
        Icon = skill.Icon;
        Quality = QualityType.QUALITY_NONE;
    }

    public SkillGroupId GroupId { get; }

    public uint LevelMin { get; }

    public uint LevelMax { get; }

    public string Name { get; }

    public Uri Icon { get; }

    public QualityType Quality { get; }

    public uint LevelCurrent
    {
        get => persistsCurrentLevel ? LocalSetting.Get(SettingKeyCurrentFromSkillType(type), LevelMin) : field;
        set => _ = persistsCurrentLevel ? SetProperty(LevelCurrent, value, v => LocalSetting.Set(SettingKeyCurrentFromSkillType(type), v)) : SetProperty(ref field, value);
    }

    public uint LevelTarget
    {
        get => LocalSetting.Get(SettingKeyTargetFromSkillType(type), LevelMax);
        set => SetProperty(LevelTarget, value, v => LocalSetting.Set(SettingKeyTargetFromSkillType(type), v));
    }

    public static CalculableSkill From(ProudSkill source, SkillType type)
    {
        return new(source, type);
    }

    public static CalculableSkill From(SkillView source, SkillType type)
    {
        return new(source, type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string SettingKeyCurrentFromSkillType(SkillType type)
    {
        return type switch
        {
            SkillType.A => SettingKeys.CultivationAvatarSkillACurrent,
            SkillType.E => SettingKeys.CultivationAvatarSkillECurrent,
            SkillType.Q => SettingKeys.CultivationAvatarSkillQCurrent,
            _ => throw HutaoException.NotSupported(),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string SettingKeyTargetFromSkillType(SkillType type)
    {
        return type switch
        {
            SkillType.A => SettingKeys.CultivationAvatarSkillATarget,
            SkillType.E => SettingKeys.CultivationAvatarSkillETarget,
            SkillType.Q => SettingKeys.CultivationAvatarSkillQTarget,
            _ => throw HutaoException.NotSupported(),
        };
    }
}