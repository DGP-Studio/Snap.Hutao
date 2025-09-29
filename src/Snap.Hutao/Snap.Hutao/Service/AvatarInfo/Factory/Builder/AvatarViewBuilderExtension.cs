// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices;
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

            ArgumentNullException.ThrowIfNull(costume.FrontIcon);
            ArgumentNullException.ThrowIfNull(costume.SideIcon);

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

    public static TBuilder SetConstellations<TBuilder>(this TBuilder builder, ImmutableArray<MetadataSkill> talents, FrozenSet<SkillId> activatedIds)
        where TBuilder : class, IAvatarViewBuilder
    {
        ImmutableArray<ConstellationView> views = talents.SelectAsArray(
            static (talent, activatedIds) => new ConstellationViewBuilder()
                .SetName(talent.Name)
                .SetIcon(SkillIconConverter.IconNameToUri(talent.Icon))
                .SetDescription(talent.Description)
                .SetIsActivated(activatedIds.Contains(talent.Id))
                .View,
            activatedIds);

        return builder.SetConstellations(views);
    }

    public static TBuilder SetConstellations<TBuilder>(this TBuilder builder, ImmutableArray<ConstellationView> constellations)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Constellations = constellations);
    }

    public static TBuilder SetElement<TBuilder>(this TBuilder builder, ElementType element)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Element = element);
    }

    public static TBuilder SetFetterLevel<TBuilder>(this TBuilder builder, uint level)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.FetterLevel = level);
    }

    public static TBuilder SetId<TBuilder>(this TBuilder builder, AvatarId id)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Id = id);
    }

    public static TBuilder SetLevelNumber<TBuilder>(this TBuilder builder, uint? levelNumber)
        where TBuilder : class, IAvatarViewBuilder
    {
        return levelNumber.TryGetValue(out uint value) ? builder.Configure(b => b.View.LevelNumber = value) : builder;
    }

    public static TBuilder SetName<TBuilder>(this TBuilder builder, string name)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Name = name);
    }

    public static TBuilder SetNameCard<TBuilder>(this TBuilder builder, Uri nameCard)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.NameCard = nameCard);
    }

    public static TBuilder SetPromoteLevel<TBuilder>(this TBuilder builder, PromoteLevel promoteLevel)
        where TBuilder : IAvatarViewBuilder
    {
        builder.View.PromoteLevel = promoteLevel;

        bool[] promoteArray = new bool[6];
        promoteArray.AsSpan(0, (int)(uint)promoteLevel).Fill(true);
        builder.View.PromoteArray = ImmutableCollectionsMarshal.AsImmutableArray(promoteArray);

        return builder;
    }

    public static TBuilder SetProperties<TBuilder>(this TBuilder builder, ImmutableArray<AvatarProperty> properties)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Properties = properties);
    }

    public static TBuilder SetQuality<TBuilder>(this TBuilder builder, QualityType quality)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Quality = quality);
    }

    public static TBuilder SetRecommendedProperties<TBuilder>(this TBuilder builder, RecommendPropertiesView recommendProperties)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.RecommendedProperties = recommendProperties);
    }

    public static TBuilder SetRecommendedProperties<TBuilder>(this TBuilder builder, RecommendProperties recommendProperties)
        where TBuilder : class, IAvatarViewBuilder
    {
        RecommendPropertiesView view = new()
        {
            SandProperties = recommendProperties.SandMainPropertyList.SelectAsArray(static prop => prop.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture)),
            GobletProperties = recommendProperties.GobletMainPropertyList.SelectAsArray(static prop => prop.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture)),
            CircletProperties = recommendProperties.CircletMainPropertyList.SelectAsArray(static prop => prop.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture)),
            SubProperties = recommendProperties.SubPropertyList.SelectAsArray(static prop => prop.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture)),
        };

        return builder.SetRecommendedProperties(view);
    }

    public static TBuilder SetReliquaries<TBuilder>(this TBuilder builder, ImmutableArray<ReliquaryView> reliquaries)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Reliquaries = reliquaries);
    }

    public static TBuilder SetWeapon<TBuilder>(this TBuilder builder, WeaponView? weapon)
        where TBuilder : class, IAvatarViewBuilder
    {
        return builder.Configure(b => b.View.Weapon = weapon);
    }
}