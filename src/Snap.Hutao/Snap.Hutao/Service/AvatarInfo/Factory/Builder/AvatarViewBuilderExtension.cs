// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;
using System.Collections.Immutable;

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
            builder.View.Icon = AvatarIconConverter.IconNameToUri(costume.FrontIcon);
            builder.View.SideIcon = AvatarIconConverter.IconNameToUri(costume.SideIcon);
        }
        else
        {
            builder.View.Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
            builder.View.SideIcon = AvatarIconConverter.IconNameToUri(avatar.SideIcon);
        }

        return builder;
    }

    public static TBuilder SetConstellations<TBuilder>(this TBuilder builder, List<Skill> talents, List<SkillId> talentIds)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetConstellations(CreateConstellations(talents, talentIds.EmptyIfNull()));

        static List<ConstellationView> CreateConstellations(List<Skill> talents, List<SkillId> talentIds)
        {
            // TODO: use builder here
            return talents.SelectList(talent => new ConstellationView()
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
        return builder.Configure(b => b.View.Constellations = constellations);
    }

    public static TBuilder SetCritScore<TBuilder>(this TBuilder builder, Dictionary<FightProperty, float>? fightPropMap)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetCritScore(ScoreCrit(fightPropMap));

        static float ScoreCrit(Dictionary<FightProperty, float>? fightPropMap)
        {
            if (fightPropMap is null or { Count: 0 })
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
        return builder.Configure(b => b.View.CritScore = critScore);
    }

    public static TBuilder SetElement<TBuilder>(this TBuilder builder, ElementType element)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Element = element);
    }

    public static TBuilder SetFetterLevel<TBuilder>(this TBuilder builder, FetterLevel? level)
        where TBuilder : IAvatarViewBuilder
    {
        if (level.TryGetValue(out FetterLevel value))
        {
            return builder.Configure(b => b.View.FetterLevel = value);
        }

        return builder;
    }

    public static TBuilder SetFetterLevel<TBuilder>(this TBuilder builder, uint level)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.FetterLevel = level);
    }

    public static TBuilder SetId<TBuilder>(this TBuilder builder, AvatarId id)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Id = id);
    }

    public static TBuilder SetLevelNumber<TBuilder>(this TBuilder builder, uint? levelNumber)
        where TBuilder : IAvatarViewBuilder
    {
        if (levelNumber.TryGetValue(out uint value))
        {
            return builder.Configure(b => b.View.LevelNumber = value);
        }

        return builder;
    }

    public static TBuilder SetName<TBuilder>(this TBuilder builder, string name)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Name = name);
    }

    public static TBuilder SetNameCard<TBuilder>(this TBuilder builder, Uri nameCard)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.NameCard = nameCard);
    }

    public static TBuilder SetProperties<TBuilder>(this TBuilder builder, List<AvatarProperty> properties)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Properties = properties);
    }

    public static TBuilder SetQuality<TBuilder>(this TBuilder builder, QualityType quality)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Quality = quality);
    }

    public static TBuilder SetReliquaries<TBuilder>(this TBuilder builder, List<ReliquaryView> reliquaries)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Reliquaries = reliquaries);
    }

    public static TBuilder SetShowcaseRefreshTimeFormat<TBuilder>(this TBuilder builder, DateTimeOffset refreshTime, Func<object?, string> format, string defaultValue)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetRefreshTimeFormat(refreshTime == DateTimeOffsetExtension.DatebaseDefaultTime ? defaultValue : format(refreshTime.ToLocalTime()));
    }

    public static TBuilder SetRefreshTimeFormat<TBuilder>(this TBuilder builder, string refreshTimeFormat)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.RefreshTimeFormat = refreshTimeFormat);
    }

    public static TBuilder SetSkills<TBuilder>(this TBuilder builder, List<ProudableSkill> proudSkills, Dictionary<SkillId, SkillLevel> skillLevels, Dictionary<SkillId, SkillLevel> extraLevels)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetSkills(CreateSkills(proudSkills, skillLevels, extraLevels));

        static List<SkillView> CreateSkills(List<ProudableSkill> proudSkills, Dictionary<SkillId, SkillLevel> skillLevels, Dictionary<SkillId, SkillLevel> extraLevels)
        {
            if (skillLevels is { Count: 0 })
            {
                return [];
            }

            Dictionary<SkillId, SkillLevel> skillNoExtraLeveledMap = new(skillLevels);

            foreach ((SkillId skillId, SkillLevel extraLevel) in extraLevels)
            {
                skillNoExtraLeveledMap.DecreaseByValue(skillId, extraLevel);
            }

            return proudSkills.SelectList(proudSkill =>
            {
                // TODO: use builder here
                return new SkillView()
                {
                    Name = proudSkill.Name,
                    Icon = SkillIconConverter.IconNameToUri(proudSkill.Icon),
                    Description = proudSkill.Description,
                    GroupId = proudSkill.GroupId,
                    Level = LevelFormat.Format(skillLevels[proudSkill.Id], extraLevels.GetValueOrDefault(proudSkill.Id)),
                    LevelNumber = skillLevels[proudSkill.Id],
                    Info = DescriptionsParametersDescriptor.Convert(proudSkill.Proud, skillNoExtraLeveledMap[proudSkill.Id]),
                };
            });
        }
    }

    public static TBuilder SetSkills<TBuilder>(this TBuilder builder, List<SkillView> skills)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Skills = skills);
    }

    public static TBuilder SetWeapon<TBuilder>(this TBuilder builder, WeaponView? weapon)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Weapon = weapon);
    }
}