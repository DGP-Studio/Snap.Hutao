// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Format;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Service.AvatarInfo.Factory.Builder;
using Snap.Hutao.Web.Enka.Model;
using System.Runtime.InteropServices;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;
using ModelAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using PropertyAvatar = Snap.Hutao.ViewModel.AvatarProperty.AvatarView;
using PropertyReliquary = Snap.Hutao.ViewModel.AvatarProperty.ReliquaryView;
using PropertyWeapon = Snap.Hutao.ViewModel.AvatarProperty.WeaponView;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

[HighQuality]
internal sealed class SummaryAvatarFactory
{
    private readonly ModelAvatarInfo avatarInfo;
    private readonly DateTimeOffset showcaseRefreshTime;
    private readonly DateTimeOffset gameRecordRefreshTime;
    private readonly DateTimeOffset calculatorRefreshTime;
    private readonly SummaryFactoryMetadataContext context;

    public SummaryAvatarFactory(SummaryFactoryMetadataContext context, EntityAvatarInfo avatarInfo)
    {
        this.context = context;
        this.avatarInfo = avatarInfo.Info;

        showcaseRefreshTime = avatarInfo.ShowcaseRefreshTime;
        gameRecordRefreshTime = avatarInfo.GameRecordRefreshTime;
        calculatorRefreshTime = avatarInfo.CalculatorRefreshTime;
    }

    public static PropertyAvatar Create(SummaryFactoryMetadataContext context, EntityAvatarInfo avatarInfo)
    {
        return new SummaryAvatarFactory(context, avatarInfo).Create();
    }

    public PropertyAvatar Create()
    {
        ReliquaryAndWeapon reliquaryAndWeapon = ProcessEquip(avatarInfo.EquipList.EmptyIfNull());
        MetadataAvatar avatar = context.IdAvatarMap[avatarInfo.AvatarId];

        PropertyAvatar propertyAvatar = new AvatarViewBuilder()
            .SetId(avatar.Id)
            .SetName(avatar.Name)
            .SetQuality(avatar.Quality)
            .SetNameCard(AvatarNameCardPicConverter.AvatarToUri(avatar))
            .SetElement(ElementNameIconConverter.ElementNameToElementType(avatar.FetterInfo.VisionBefore))
            .SetConstellations(avatar.SkillDepot.Talents, avatarInfo.TalentIdList)
            .SetSkills(avatarInfo.SkillLevelMap, avatarInfo.ProudSkillExtraLevelMap, avatar.SkillDepot.CompositeSkillsNoInherents())
            .SetFetterLevel(avatarInfo.FetterInfo?.ExpLevel)
            .SetProperties(SummaryAvatarProperties.Create(avatarInfo.FightPropMap))
            .SetCritScore(avatarInfo.FightPropMap)
            .SetLevelNumber(avatarInfo.PropMap?[PlayerProperty.PROP_LEVEL].Value)
            .SetWeapon(reliquaryAndWeapon.Weapon)
            .SetReliquaries(reliquaryAndWeapon.Reliquaries)
            .SetScore(reliquaryAndWeapon.Reliquaries.Sum(r => r.Score))
            .SetShowcaseRefreshTimeFormat(showcaseRefreshTime, SH.FormatServiceAvatarInfoSummaryShowcaseRefreshTimeFormat, SH.ServiceAvatarInfoSummaryShowcaseNotRefreshed)
            .SetGameRecordRefreshTimeFormat(gameRecordRefreshTime, SH.FormatServiceAvatarInfoSummaryGameRecordRefreshTimeFormat, SH.ServiceAvatarInfoSummaryGameRecordNotRefreshed)
            .SetCalculatorRefreshTimeFormat(calculatorRefreshTime, SH.FormatServiceAvatarInfoSummaryCalculatorRefreshTimeFormat, SH.ServiceAvatarInfoSummaryCalculatorNotRefreshed)
            .ApplyCostumeIconOrDefault(avatarInfo, avatar)
            .AvatarView;

        return propertyAvatar;
    }

    private ReliquaryAndWeapon ProcessEquip(List<Equip> equipments)
    {
        List<PropertyReliquary> reliquaryList = [];
        PropertyWeapon? weapon = null;

        foreach (ref readonly Equip equip in CollectionsMarshal.AsSpan(equipments))
        {
            switch (equip.Flat.ItemType)
            {
                case ItemType.ITEM_RELIQUARY:
                    reliquaryList.Add(SummaryReliquaryFactory.Create(context, avatarInfo, equip));
                    break;
                case ItemType.ITEM_WEAPON:
                    weapon = CreateWeapon(equip);
                    break;
            }
        }

        return new(reliquaryList, weapon);
    }

    private PropertyWeapon CreateWeapon(Equip equip)
    {
        MetadataWeapon weapon = context.IdWeaponMap[equip.ItemId];

        // AffixMap can be null when it's a white weapon.
        ArgumentNullException.ThrowIfNull(equip.Weapon);
        uint affixLevel = equip.Weapon.AffixMap?.SingleOrDefault().Value ?? 0U;

        WeaponStat? mainStat = equip.Flat.WeaponStats?.ElementAtOrDefault(0);
        WeaponStat? subStat = equip.Flat.WeaponStats?.ElementAtOrDefault(1);

        NameDescription subProperty;
        if (subStat is null)
        {
            subProperty = NameDescription.Default;
        }
        else
        {
            float statValue = subStat.AppendPropId.GetFormatMethod() is FormatMethod.Percent
                ? subStat.StatValue / 100F
                : subStat.StatValue;
            subProperty = FightPropertyFormat.ToNameDescription(subStat.AppendPropId, statValue);
        }

        ArgumentNullException.ThrowIfNull(equip.Weapon);

        return new()
        {
            // NameIconDescription
            Name = weapon.Name,
            Icon = EquipIconConverter.IconNameToUri(weapon.Icon),
            Description = weapon.Description,

            // EquipBase
            Level = $"Lv.{equip.Weapon.Level.Value}",
            Quality = weapon.Quality,
            MainProperty = mainStat is not null
                ? FightPropertyFormat.ToNameValue(mainStat.AppendPropId, mainStat.StatValue)
                : NameValueDefaults.String,

            // Weapon
            Id = weapon.Id,
            LevelNumber = equip.Weapon.Level,
            SubProperty = subProperty,
            AffixLevelNumber = affixLevel + 1,
            AffixName = weapon.Affix?.Name ?? string.Empty,
            AffixDescription = weapon.Affix?.Descriptions.Single(a => a.Level == affixLevel).Description ?? string.Empty,
        };
    }

    private readonly struct ReliquaryAndWeapon
    {
        public readonly List<PropertyReliquary> Reliquaries;
        public readonly PropertyWeapon? Weapon;

        public ReliquaryAndWeapon(List<PropertyReliquary> reliquaries, PropertyWeapon? weapon)
        {
            Reliquaries = reliquaries;
            Weapon = weapon;
        }
    }
}