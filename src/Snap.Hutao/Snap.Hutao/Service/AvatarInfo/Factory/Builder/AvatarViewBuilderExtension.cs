// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Collections.Immutable;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataCostume = Snap.Hutao.Model.Metadata.Avatar.Costume;
using MetadataSkill = Snap.Hutao.Model.Metadata.Avatar.Skill;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class AvatarViewBuilderExtension
{
    public static TBuilder SetCostumeIconOrDefault<TBuilder>(this TBuilder builder, DetailedCharacter detailedCharacter, MetadataAvatar avatar)
        where TBuilder : IAvatarViewBuilder
    {
        if (detailedCharacter.Costumes is [{ Id: { } id }, ..])
        {
            MetadataCostume costume = avatar.Costumes.Single(c => c.Id == id);

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

    public static TBuilder SetConstellations<TBuilder>(this TBuilder builder, ImmutableArray<MetadataSkill> talents, List<SkillId> talentIds)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetConstellations(CreateConstellations(talents, talentIds.EmptyIfNull()));

        static ImmutableArray<ConstellationView> CreateConstellations(ImmutableArray<MetadataSkill> talents, List<SkillId> talentIds)
        {
            // TODO: use builder here
            return talents.SelectArray(talent => new ConstellationView
            {
                Name = talent.Name,
                Icon = SkillIconConverter.IconNameToUri(talent.Icon),
                Description = talent.Description,
                IsActivated = talentIds.Contains(talent.Id),
            });
        }
    }

    public static TBuilder SetConstellations<TBuilder>(this TBuilder builder, ImmutableArray<ConstellationView> constellations)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Constellations = constellations);
    }

    public static TBuilder SetElement<TBuilder>(this TBuilder builder, ElementType element)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Element = element);
    }

    public static TBuilder SetFetterLevel<TBuilder>(this TBuilder builder, FetterLevel? level)
        where TBuilder : IAvatarViewBuilder
    {
        return level.TryGetValue(out FetterLevel value) ? builder.Configure(b => b.View.FetterLevel = value) : builder;
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
        return levelNumber.TryGetValue(out uint value) ? builder.Configure(b => b.View.LevelNumber = value) : builder;
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

    public static TBuilder SetRecommendedProperties<TBuilder>(this TBuilder builder, RecommandPropertiesView recommandProperties)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.RecommendedProperties = recommandProperties);
    }

    public static unsafe TBuilder SetRecommendedProperties<TBuilder>(this TBuilder builder, RecommendProperties recommandProperties)
        where TBuilder : IAvatarViewBuilder
    {
        RecommandPropertiesView view = new()
        {
            SandProperties = recommandProperties.SandMainPropertyList.SelectList(&FightPropertyExtension.GetLocalizedDescription),
            GobletProperties = recommandProperties.GobletMainPropertyList.SelectList(&FightPropertyExtension.GetLocalizedDescription),
            CircletProperties = recommandProperties.CircletMainPropertyList.SelectList(&FightPropertyExtension.GetLocalizedDescription),
            SubProperties = recommandProperties.SubPropertyList.SelectList(&FightPropertyExtension.GetLocalizedDescription),
        };

        return builder.SetRecommendedProperties(view);
    }

    public static TBuilder SetReliquaries<TBuilder>(this TBuilder builder, List<ReliquaryView> reliquaries)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Reliquaries = reliquaries);
    }

    public static TBuilder SetRefreshTimeFormat<TBuilder>(this TBuilder builder, DateTimeOffset refreshTime, Func<object?, string> format, string defaultValue)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetRefreshTimeFormat(refreshTime == DateTimeOffsetExtension.DatebaseDefaultTime ? defaultValue : format(refreshTime.ToLocalTime()));
    }

    public static TBuilder SetRefreshTimeFormat<TBuilder>(this TBuilder builder, string refreshTimeFormat)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.RefreshTimeFormat = refreshTimeFormat);
    }

    public static TBuilder SetSkills<TBuilder>(this TBuilder builder, ImmutableArray<ProudableSkill> proudSkills, Dictionary<SkillId, SkillLevel> skillLevels, Dictionary<SkillId, SkillLevel> extraLevels)
        where TBuilder : IAvatarViewBuilder
    {
        return builder.SetSkills(CreateSkills(proudSkills, skillLevels, extraLevels));

        static ImmutableArray<SkillView> CreateSkills(ImmutableArray<ProudableSkill> proudSkills, Dictionary<SkillId, SkillLevel> skillLevels, Dictionary<SkillId, SkillLevel> extraLevels)
        {
            if (skillLevels is { Count: 0 })
            {
                return [];
            }

            Dictionary<SkillId, SkillLevel> nonExtraLeveledSkills = new(skillLevels);

            // TODO: Test 达达利亚技能的影响
            foreach ((SkillId skillId, SkillLevel extraLevel) in extraLevels)
            {
                nonExtraLeveledSkills.DecreaseByValue(skillId, extraLevel);
            }

            return proudSkills.SelectArray(proudSkill => new SkillView
            {
                Name = proudSkill.Name,
                Icon = SkillIconConverter.IconNameToUri(proudSkill.Icon),
                Description = proudSkill.Description,
                GroupId = proudSkill.GroupId,
                Level = LevelFormat.Format(nonExtraLeveledSkills[proudSkill.Id], extraLevels.GetValueOrDefault(proudSkill.Id)),
                LevelNumber = nonExtraLeveledSkills[proudSkill.Id],
                Info = DescriptionsParametersDescriptor.Convert(proudSkill.Proud, skillLevels[proudSkill.Id]),
            });
        }
    }

    public static TBuilder SetSkills<TBuilder>(this TBuilder builder, ImmutableArray<SkillView> skills)
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