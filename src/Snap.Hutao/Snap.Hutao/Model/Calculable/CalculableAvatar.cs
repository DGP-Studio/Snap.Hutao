// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Calculable;

internal sealed partial class CalculableAvatar : ObservableObject, ICalculableAvatar
{
    // Only persists current level for non-view avatars
    private readonly bool persistsCurrentLevel;

    private CalculableAvatar(Avatar avatar)
    {
        persistsCurrentLevel = true;

        AvatarId = avatar.Id;
        LevelMin = 1;
        LevelMax = avatar.MaxLevel;
        Skills = avatar.SkillDepot.CompositeSkillsNoInherents.SelectAsArray(static (p, i) => p.ToCalculable((SkillType)i));
        Name = avatar.Name;
        Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
        Quality = avatar.Quality;

        LevelCurrent = LevelMin;
    }

    private CalculableAvatar(AvatarView avatar)
    {
        persistsCurrentLevel = false;

        AvatarId = avatar.Id;
        LevelMin = avatar.LevelNumber;
        LevelMax = Avatar.GetMaxLevel();
        Skills = avatar.Skills.SelectAsArray(static (s, i) => s.ToCalculable((SkillType)i));
        Name = avatar.Name;
        Icon = avatar.Icon;
        Quality = avatar.Quality;

        IsPromoted = BaseValueInfoConverter.GetPromoted(avatar.LevelNumber, avatar.PromoteLevel);

        LevelCurrent = LevelMin;
    }

    public AvatarId AvatarId { get; }

    public uint LevelMin { get; }

    public uint LevelMax { get; }

    public ImmutableArray<ICalculableSkill> Skills { get; }

    public string Name { get; }

    public Uri Icon { get; }

    public QualityType Quality { get; }

    [ObservableProperty]
    public partial bool IsPromoted { get; set; }

    public PromoteLevel PromoteLevel { get => BaseValueInfoConverter.GetPromoteLevel(LevelCurrent, LevelMax, IsPromoted); }

    public bool IsPromotionAvailable
    {
        get => LevelCurrent is 20U or 40U or 50U or 60U or 70U or 80U;
    }

    public uint LevelCurrent
    {
        get => persistsCurrentLevel ? LocalSetting.Get(SettingKeys.CultivationAvatarLevelCurrent, LevelMin) : field;
        set
        {
            if (persistsCurrentLevel ? SetProperty(LevelCurrent, value, v => LocalSetting.Set(SettingKeys.CultivationAvatarLevelCurrent, v)) : SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(IsPromotionAvailable));
                IsPromoted = false;
            }
        }
    }

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