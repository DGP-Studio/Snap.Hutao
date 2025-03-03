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
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Calculable;

internal sealed partial class CalculableAvatar : ObservableObject, ICalculableAvatar
{
    // Only persists current level for non-view avatars
    private readonly bool persistsLevel;

    private CalculableAvatar(Avatar avatar)
    {
        persistsLevel = true;

        AvatarId = avatar.Id;
        LevelMin = 1;
        LevelMax = avatar.MaxLevel;
        Skills = avatar.SkillDepot.CompositeSkillsNoInherents.SelectAsArray((p, i) => p.ToCalculable((SkillType)i));
        Name = avatar.Name;
        Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
        Quality = avatar.Quality;
        PromoteLevel = 0;

        LevelCurrent = LevelMin;
    }

    private CalculableAvatar(AvatarView avatar)
    {
        persistsLevel = false;

        AvatarId = avatar.Id;
        LevelMin = avatar.LevelNumber;
        LevelMax = Avatar.GetMaxLevel();
        Skills = avatar.Skills.SelectAsArray((s, i) => s.ToCalculable((SkillType)i));
        Name = avatar.Name;
        Icon = avatar.Icon;
        Quality = avatar.Quality;

        // PromoteLevel must be set after IsPromoted
        IsPromoted = BaseValueInfoConverter.GetPromoted(avatar.LevelNumber, avatar.PromoteLevel);
        PromoteLevel = avatar.PromoteLevel;

        LevelCurrent = LevelMin;
    }

    public AvatarId AvatarId { get; }

    public uint LevelMin { get; }

    public uint LevelMax { get; }

    public ImmutableArray<ICalculableSkill> Skills { get; }

    public string Name { get; }

    public Uri Icon { get; }

    public QualityType Quality { get; }

    public PromoteLevel PromoteLevel { get; private set; }

    public bool IsPromotionAvailable
    {
        get => LevelCurrent is 20U or 40U or 50U or 60U or 70U or 80U;
    }

    public uint LevelCurrent
    {
        get => persistsLevel ? LocalSetting.Get(SettingKeys.CultivationAvatarLevelCurrent, LevelMin) : field;
        set
        {
            if (persistsLevel ? SetProperty(LevelCurrent, value, v => LocalSetting.Set(SettingKeys.CultivationAvatarLevelCurrent, v)) : SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(IsPromotionAvailable));
                IsPromoted = false;
                PromoteLevel = value switch
                {
                    > 0 and <= 20 => 0,
                    <= 40 => 1,
                    <= 50 => 2,
                    <= 60 => 3,
                    <= 70 => 4,
                    <= 80 => 5,
                    <= 90 => 6,
                    _ => throw HutaoException.InvalidOperation("Invalid avatar level."),
                };
            }
        }
    }

    public uint LevelTarget
    {
        get => LocalSetting.Get(SettingKeys.CultivationAvatarLevelTarget, LevelMax);
        set => SetProperty(LevelTarget, value, v => LocalSetting.Set(SettingKeys.CultivationAvatarLevelTarget, v));
    }

    public bool IsPromoted
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value)
            {
                PromoteLevel++;
            }
        }
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