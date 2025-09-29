// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.AvatarInfo.Factory.Builder;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.ViewModel.Wiki;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Collections.Frozen;
using System.Collections.Immutable;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

internal sealed class SummaryAvatarFactory
{
    private readonly DetailedCharacter character;
    private readonly SummaryFactoryMetadataContext context;

    public SummaryAvatarFactory(SummaryFactoryMetadataContext context, EntityAvatarInfo avatarInfo)
    {
        ArgumentNullException.ThrowIfNull(avatarInfo.Info2);

        this.context = context;
        character = avatarInfo.Info2;
    }

    public static AvatarView Create(SummaryFactoryMetadataContext context, EntityAvatarInfo avatarInfo)
    {
        return new SummaryAvatarFactory(context, avatarInfo).Create();
    }

    public AvatarView Create()
    {
        MetadataAvatar avatar = context.GetAvatar(character.Base.Id);

        ProcessConstellations(
            avatar.SkillDepot,
            character.Constellations,
            out FrozenSet<SkillId> activatedConstellations,
            out FrozenDictionary<SkillId, SkillLevel> extraLevels);

        ImmutableArray<AvatarProperty> properties = character.SelectedProperties.SelectAsArray(FightPropertyFormat.ToAvatarProperty);
        properties.SortInPlace(InGameAvatarPropertyComparer.Shared);

        AvatarView propertyAvatar = new AvatarViewBuilder()
            .SetId(avatar.Id)
            .SetName(avatar.Name)
            .SetQuality(avatar.Quality)
            .SetNameCard(AvatarNameCardPicConverter.IconNameToUri(avatar.NameCard.PicturePrefix))
            .SetElement(ElementNameIconConverter.ElementNameToElementType(avatar.FetterInfo.VisionBefore))
            .SetConstellations(avatar.SkillDepot.Talents, activatedConstellations)
            .SetSkills(avatar.SkillDepot.CompositeSkillsNoInherents, character.Skills.ToFrozenDictionary(s => s.SkillId, s => s.Level), extraLevels)
            .SetFetterLevel(character.Base.Fetter)
            .SetProperties(properties)
            .SetLevelNumber(character.Base.Level)
            .SetWeapon(CreateWeapon(character.Weapon))
            .SetRecommendedProperties(character.RecommendRelicProperty.RecommendProperties)
            .SetReliquaries(character.Relics.SelectAsArray(static (relic, context) => SummaryReliquaryFactory.Create(context, relic), context))
            .SetCostumeIconOrDefault(character, avatar)
            .SetPromoteLevel(character.Base.PromoteLevel)
            .View;

        return propertyAvatar;
    }

    private static void ProcessConstellations(
        SkillDepot depot,
        ImmutableArray<Constellation> dataConstellations,
        out FrozenSet<SkillId> activatedConstellationIds,
        out FrozenDictionary<SkillId, SkillLevel> extraLevels)
    {
        HashSet<SkillId> constellationIds = [];
        Dictionary<SkillId, SkillLevel> levels = [];

        // 达达利亚天赋：普攻技能等级+1
        if (depot.Inherents is [_, _, { } inherent] && inherent.GroupId == 3323U)
        {
            levels.Add(depot.CompositeSkillsNoInherents[0].Id, 1);
        }

        Verify.Operation(depot.Talents.Length == dataConstellations.Length, "Constellation count mismatch");
        foreach ((Model.Metadata.Avatar.Skill metaConstellation, Constellation dataConstellation) in depot.Talents.Zip(dataConstellations))
        {
            // Constellations are activated in order, so if the current constellation is
            // not activated, all the subsequent constellations will not be activated.
            if (!dataConstellation.IsActived)
            {
                break;
            }

            constellationIds.Add(dataConstellation.Id);

            if (metaConstellation.ExtraLevel is { } extraLevel)
            {
                int index = extraLevel.Index switch
                {
                    ExtraLevelIndexKind.NormalAttack => 0,
                    ExtraLevelIndexKind.ElementalSkill => 1,
                    ExtraLevelIndexKind.ElementalBurst => 2,
                    _ => throw HutaoException.NotSupported("Unexpected extra level index."),
                };

                levels.Add(depot.CompositeSkillsNoInherents[index].Id, extraLevel.Level);
            }
        }

        activatedConstellationIds = constellationIds.ToFrozenSet();
        extraLevels = levels.ToFrozenDictionary();
    }

    private WeaponView CreateWeapon(DetailedWeapon detailedWeapon)
    {
        MetadataWeapon metadataWeapon = context.GetWeapon(detailedWeapon.Id);

        BaseValueInfoMetadataContext baseValueContext = new()
        {
            GrowCurveMap = context.LevelDictionaryWeaponGrowCurveMap,
            PromoteMap = context.IdDictionaryWeaponLevelPromoteMap[metadataWeapon.PromoteId],
        };

        ImmutableArray<NameValue<string>> baseValues = BaseValueInfoConverter.ToNameValues(metadataWeapon.GrowCurves.ToPropertyCurveValues(), detailedWeapon.Level, detailedWeapon.PromoteLevel, baseValueContext);

        return new WeaponViewBuilder()
            .SetName(metadataWeapon.Name)
            .SetIcon(EquipIconConverter.IconNameToUri(metadataWeapon.Icon))
            .SetDescription(metadataWeapon.Description)
            .SetLevel(LevelFormat.Format(detailedWeapon.Level))
            .SetQuality(metadataWeapon.Quality)
            .SetEquipType(EquipType.EQUIP_WEAPON)
            .SetId(metadataWeapon.Id)
            .SetLevelNumber(detailedWeapon.Level)
            .SetMainProperty(baseValues.ElementAtOrDefault(0))
            .SetSubProperty(baseValues.ElementAtOrDefault(1))
            .SetAffixLevelNumber(detailedWeapon.AffixLevel)
            .SetAffixName(metadataWeapon.Affix?.Name)
            .SetAffixDescription(metadataWeapon.Affix?.Descriptions.Single(a => a.Level == (detailedWeapon.AffixLevel - 1)).Description)
            .SetWeaponType(metadataWeapon.WeaponType)
            .SetPromoteLevel(detailedWeapon.PromoteLevel)
            .View;
    }
}