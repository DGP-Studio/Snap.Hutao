// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.AvatarInfo.Factory.Builder;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.ViewModel.Wiki;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Collections.Immutable;
using System.Globalization;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

[HighQuality]
internal sealed class SummaryAvatarFactory
{
    private readonly DetailedCharacter character;
    private readonly DateTimeOffset refreshTime;
    private readonly SummaryFactoryMetadataContext context;

    public SummaryAvatarFactory(SummaryFactoryMetadataContext context, EntityAvatarInfo avatarInfo)
    {
        ArgumentNullException.ThrowIfNull(avatarInfo.Info2);

        this.context = context;
        character = avatarInfo.Info2;
        refreshTime = avatarInfo.RefreshTime;
    }

    public static AvatarView Create(SummaryFactoryMetadataContext context, EntityAvatarInfo avatarInfo)
    {
        return new SummaryAvatarFactory(context, avatarInfo).Create();
    }

    public unsafe AvatarView Create()
    {
        MetadataAvatar avatar = context.IdAvatarMap[character.Base.Id];
        ProcessConstellations(avatar.SkillDepot, character.Constellations, out List<SkillId> activatedConstellations, out Dictionary<SkillId, SkillLevel> extraLevels);

        AvatarView propertyAvatar = new AvatarViewBuilder()
            .SetId(avatar.Id)
            .SetName(avatar.Name)
            .SetQuality(avatar.Quality)
            .SetNameCard(AvatarNameCardPicConverter.IconNameToUri(avatar.NameCard.PicturePrefix))
            .SetElement(ElementNameIconConverter.ElementNameToElementType(avatar.FetterInfo.VisionBefore))
            .SetConstellations(avatar.SkillDepot.Talents, activatedConstellations)
            .SetSkills(avatar.SkillDepot.CompositeSkillsNoInherents, character.Skills.ToDictionary(s => s.SkillId, s => s.Level), extraLevels)
            .SetFetterLevel(character.Base.Fetter)
            .SetProperties(character.SelectedProperties.SortBy(p => p.PropertyType, InGameFightPropertyComparer.Shared).SelectList(&FightPropertyFormat.ToAvatarProperty))
            .SetLevelNumber(character.Base.Level)
            .SetWeapon(CreateWeapon(character.Weapon))
            .SetRecommendedProperties(character.RecommendRelicProperty.RecommendProperties)
            .SetReliquaries(character.Relics.SelectList(relic => SummaryReliquaryFactory.Create(context, relic)))
            .SetRefreshTimeFormat(refreshTime, obj => string.Format(CultureInfo.CurrentCulture, "{0:MM-dd HH:mm}", obj), SH.ServiceAvatarInfoSummaryNotRefreshed)
            .SetCostumeIconOrDefault(character, avatar)
            .View;

        return propertyAvatar;
    }

    private static void ProcessConstellations(SkillDepot depot, List<Constellation> constellations, out List<SkillId> activatedConstellationIds, out Dictionary<SkillId, SkillLevel> extraLevels)
    {
        activatedConstellationIds = [];
        extraLevels = [];

        foreach (RefValueTuple<Model.Metadata.Avatar.Skill, Constellation> tuple in depot.Talents.ZipList(constellations))
        {
            ref readonly Constellation dataConstellation = ref tuple.Item2;

            // Constellations are activated in order
            if (!dataConstellation.IsActived)
            {
                break;
            }

            activatedConstellationIds.Add(dataConstellation.Id);

            ref readonly Model.Metadata.Avatar.Skill metaConstellation = ref tuple.Item1;
            if (metaConstellation.ExtraLevel is { } extraLevel)
            {
                int index = extraLevel.Index switch
                {
                    ExtraLevelIndexKind.NormalAttack => 0,
                    ExtraLevelIndexKind.ElementalSkill => 1,
                    ExtraLevelIndexKind.ElementalBurst => 2,
                    _ => throw HutaoException.NotSupported(),
                };

                extraLevels.Add(depot.CompositeSkillsNoInherents[index].Id, extraLevel.Level);
            }
        }
    }

    private WeaponView CreateWeapon(DetailedWeapon detailedWeapon)
    {
        MetadataWeapon metadataWeapon = context.IdWeaponMap[detailedWeapon.Id];

        ImmutableArray<NameValue<string>> baseValues = metadataWeapon.GrowCurves.Select(growCurve => BaseValueInfoFormat.ToNameValue(
            PropertyCurveValue.From(growCurve),
            detailedWeapon.Level,
            detailedWeapon.PromoteLevel,
            context.LevelDictionaryWeaponGrowCurveMap,
            context.IdDictionaryWeaponLevelPromoteMap[metadataWeapon.PromoteId])).ToImmutableArray();

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
            .View;
    }
}