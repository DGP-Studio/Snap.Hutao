// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction.Extension;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class AvatarViewBuilderExtension
{
    public static TBuilder SetCostumeIconOrDefault<TBuilder>(this TBuilder builder, Web.Enka.Model.AvatarInfo avatarInfo, Avatar avatar)
        where TBuilder : IAvatarViewBuilder
    {
        if (avatarInfo.CostumeId.TryGetValue(out CostumeId id))
        {
            Costume costume = avatar.Costumes.Single(c => c.Id == id);

            // Set to costume icon
            builder.AvatarView.Icon = AvatarIconConverter.IconNameToUri(costume.FrontIcon);
            builder.AvatarView.SideIcon = AvatarIconConverter.IconNameToUri(costume.SideIcon);
        }
        else
        {
            builder.AvatarView.Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
            builder.AvatarView.SideIcon = AvatarIconConverter.IconNameToUri(avatar.SideIcon);
        }

        return builder;
    }

    public static TBuilder SetCalculatorRefreshTimeFormat<TBuilder>(this TBuilder builder, DateTimeOffset refreshTime, Func<object?, string> format, string defaultValue)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetCalculatorRefreshTimeFormat(refreshTime == DateTimeOffsetExtension.DatebaseDefaultTime ? defaultValue : format(refreshTime.ToLocalTime()));
    }

    public static TBuilder SetCalculatorRefreshTimeFormat<TBuilder>(this TBuilder builder, string calculatorRefreshTimeFormat)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.CalculatorRefreshTimeFormat = calculatorRefreshTimeFormat);
    }

    public static TBuilder SetConstellations<TBuilder>(this TBuilder builder, List<Skill> talents, List<SkillId>? talentIds)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetConstellations(CreateConstellations(talents, talentIds.EmptyIfNull()));

        static List<ConstellationView> CreateConstellations(List<Skill> talents, List<SkillId> talentIds)
        {
            // TODO: use builder here
            return talents.SelectList(talent => new ViewModel.AvatarProperty.ConstellationView()
            {
                Name = talent.Name,
                Icon = SkillIconConverter.IconNameToUri(talent.Icon),
                Description = talent.Description,
                IsActivated = talentIds.Contains(talent.Id),
            });
        }
    }

    public static TBuilder SetConstellations<TBuilder>(this TBuilder builder, List<ConstellationView> constellations)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Constellations = constellations);
    }

    public static TBuilder SetCritScore<TBuilder>(this TBuilder builder, Dictionary<FightProperty, float>? fightPropMap)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetCritScore(ScoreCrit(fightPropMap));

        static float ScoreCrit(Dictionary<FightProperty, float>? fightPropMap)
        {
            if (fightPropMap.IsNullOrEmpty())
            {
                return 0F;
            }

            float cr = fightPropMap[FightProperty.FIGHT_PROP_CRITICAL];
            float cd = fightPropMap[FightProperty.FIGHT_PROP_CRITICAL_HURT];

            return 100 * ((cr * 2) + cd);
        }
    }

    public static TBuilder SetCritScore<TBuilder>(this TBuilder builder, float critScore)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetCritScore($"{critScore:F2}");
    }

    public static TBuilder SetCritScore<TBuilder>(this TBuilder builder, string critScore)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.CritScore = critScore);
    }

    public static TBuilder SetElement<TBuilder>(this TBuilder builder, ElementType element)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Element = element);
    }

    public static TBuilder SetFetterLevel<TBuilder>(this TBuilder builder, FetterLevel? level)
        where TBuilder : IAvatarViewBuilder
    {
        if (level.TryGetValue(out FetterLevel value))
        {
            return builder.Configure(b => b.AvatarView.FetterLevel = value);
        }

        return builder;
    }

    public static TBuilder SetFetterLevel<TBuilder>(this TBuilder builder, uint level)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.FetterLevel = level);
    }

    public static TBuilder SetGameRecordRefreshTimeFormat<TBuilder>(this TBuilder builder, DateTimeOffset refreshTime, Func<object?, string> format, string defaultValue)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetGameRecordRefreshTimeFormat(refreshTime == DateTimeOffsetExtension.DatebaseDefaultTime ? defaultValue : format(refreshTime.ToLocalTime()));
    }

    public static TBuilder SetGameRecordRefreshTimeFormat<TBuilder>(this TBuilder builder, string gameRecordRefreshTimeFormat)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.GameRecordRefreshTimeFormat = gameRecordRefreshTimeFormat);
    }

    public static TBuilder SetId<TBuilder>(this TBuilder builder, AvatarId id)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Id = id);
    }

    public static TBuilder SetLevelNumber<TBuilder>(this TBuilder builder, uint? levelNumber)
        where TBuilder : IAvatarViewBuilder
    {
        if (levelNumber.TryGetValue(out uint value))
        {
            return builder.Configure(b => b.AvatarView.LevelNumber = value);
        }

        return builder;
    }

    public static TBuilder SetName<TBuilder>(this TBuilder builder, string name)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Name = name);
    }

    public static TBuilder SetNameCard<TBuilder>(this TBuilder builder, Uri nameCard)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.NameCard = nameCard);
    }

    public static TBuilder SetProperties<TBuilder>(this TBuilder builder, List<AvatarProperty> properties)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Properties = properties);
    }

    public static TBuilder SetQuality<TBuilder>(this TBuilder builder, QualityType quality)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Quality = quality);
    }

    public static TBuilder SetReliquaries<TBuilder>(this TBuilder builder, List<ReliquaryView> reliquaries)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Reliquaries = reliquaries);
    }

    public static TBuilder SetScore<TBuilder>(this TBuilder builder, float score)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetScore($"{score:F2}");
    }

    public static TBuilder SetScore<TBuilder>(this TBuilder builder, string score)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Score = score);
    }

    public static TBuilder SetShowcaseRefreshTimeFormat<TBuilder>(this TBuilder builder, DateTimeOffset refreshTime, Func<object?, string> format, string defaultValue)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetShowcaseRefreshTimeFormat(refreshTime == DateTimeOffsetExtension.DatebaseDefaultTime ? defaultValue : format(refreshTime.ToLocalTime()));
    }

    public static TBuilder SetShowcaseRefreshTimeFormat<TBuilder>(this TBuilder builder, string showcaseRefreshTimeFormat)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.ShowcaseRefreshTimeFormat = showcaseRefreshTimeFormat);
    }

    public static TBuilder SetSkills<TBuilder>(this TBuilder builder, Dictionary<SkillId, SkillLevel>? skillLevelMap, Dictionary<SkillGroupId, SkillLevel>? proudSkillExtraLevelMap, List<ProudableSkill> proudSkills)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetSkills(CreateSkills(skillLevelMap, proudSkillExtraLevelMap, proudSkills));

        static List<SkillView> CreateSkills(Dictionary<SkillId, SkillLevel>? skillLevelMap, Dictionary<SkillGroupId, SkillLevel>? proudSkillExtraLevelMap, List<ProudableSkill> proudSkills)
        {
            if (skillLevelMap.IsNullOrEmpty())
            {
                return [];
            }

            Dictionary<SkillId, SkillLevel> skillExtraLeveledMap = new(skillLevelMap);

            if (proudSkillExtraLevelMap is not null)
            {
                foreach ((SkillGroupId groupId, SkillLevel extraLevel) in proudSkillExtraLevelMap)
                {
                    skillExtraLeveledMap.IncreaseValue(proudSkills.Single(p => p.GroupId == groupId).Id, extraLevel);
                }
            }

            return proudSkills.SelectList(proudableSkill =>
            {
                SkillId skillId = proudableSkill.Id;

                // TODO: use builder here
                return new SkillView()
                {
                    Name = proudableSkill.Name,
                    Icon = SkillIconConverter.IconNameToUri(proudableSkill.Icon),
                    Description = proudableSkill.Description,

                    GroupId = proudableSkill.GroupId,
                    LevelNumber = skillLevelMap[skillId],
                    Info = DescriptionsParametersDescriptor.Convert(proudableSkill.Proud, skillExtraLeveledMap[skillId]),
                };
            });
        }
    }

    public static TBuilder SetSkills<TBuilder>(this TBuilder builder, List<SkillView> skills)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Skills = skills);
    }

    public static TBuilder SetWeapon<TBuilder>(this TBuilder builder, WeaponView? weapon)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.AvatarView.Weapon = weapon);
    }
}