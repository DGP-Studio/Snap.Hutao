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

internal sealed class CalculableAvatar
    : ObservableObject,
    ICalculableAvatar,
    IMappingFrom<CalculableAvatar, Avatar>,
    IMappingFrom<CalculableAvatar, AvatarView>
{
    private CalculableAvatar(Avatar avatar)
    {
        AvatarId = avatar.Id;
        LevelMin = 1;
        LevelMax = avatar.MaxLevel;
        Skills = avatar.SkillDepot.CompositeSkillsNoInherents().SelectList((p, i) => p.ToCalculable((SkillType)i));
        Name = avatar.Name;
        Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
        Quality = avatar.Quality;
    }

    private CalculableAvatar(AvatarView avatar)
    {
        AvatarId = avatar.Id;
        LevelMin = avatar.LevelNumber;
        LevelMax = Avatar.GetMaxLevel();
        Skills = avatar.Skills.SelectList((s, i) => s.ToCalculable((SkillType)i));
        Name = avatar.Name;
        Icon = avatar.Icon;
        Quality = avatar.Quality;
    }

    public AvatarId AvatarId { get; }

    public uint LevelMin { get; }

    public uint LevelMax { get; }

    public List<ICalculableSkill> Skills { get; }

    public string Name { get; }

    public Uri Icon { get; }

    public QualityType Quality { get; }

    public uint LevelCurrent
    {
        get => LocalSetting.Get(SettingKeys.CultivationAvatarLevelCurrent, LevelMin);
        set => SetProperty(LevelCurrent, value, v => LocalSetting.Set(SettingKeys.CultivationAvatarLevelCurrent, v));
    }

    /// <inheritdoc/>
    public uint LevelTarget
    {
        get => LocalSetting.Get(SettingKeys.CultivationAvatarLevelTarget, LevelMax);
        set => SetProperty(LevelTarget, value, v => LocalSetting.Set(SettingKeys.CultivationAvatarLevelTarget, v));
    }

    public static CalculableAvatar From(Avatar source)
    {
        return new(source);
    }

    public static CalculableAvatar From(AvatarView source)
    {
        return new(source);
    }
}