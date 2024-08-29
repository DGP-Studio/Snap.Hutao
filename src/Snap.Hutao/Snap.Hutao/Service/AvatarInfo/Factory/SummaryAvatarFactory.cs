// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Format;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.AvatarInfo.Factory.Builder;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Runtime.InteropServices;
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
        this.context = context;
        character = avatarInfo.Info2;

        refreshTime = avatarInfo.RefreshTime;
    }

    public static AvatarView Create(SummaryFactoryMetadataContext context, EntityAvatarInfo avatarInfo)
    {
        return new SummaryAvatarFactory(context, avatarInfo).Create();
    }

    public AvatarView Create()
    {
        ReliquaryAndWeapon reliquaryAndWeapon = ProcessEquip(character.EquipList.EmptyIfNull());
        MetadataAvatar avatar = context.IdAvatarMap[character.AvatarId];

        AvatarView propertyAvatar = new AvatarViewBuilder()
            .SetId(avatar.Id)
            .SetName(avatar.Name)
            .SetQuality(avatar.Quality)
            .SetNameCard(AvatarNameCardPicConverter.AvatarToUri(avatar))
            .SetElement(ElementNameIconConverter.ElementNameToElementType(avatar.FetterInfo.VisionBefore))
            .SetConstellations(avatar.SkillDepot.Talents, character.TalentIdList)
            .SetSkills(character.SkillLevelMap, character.ProudSkillExtraLevelMap, avatar.SkillDepot.CompositeSkillsNoInherents())
            .SetFetterLevel(character.FetterInfo?.ExpLevel)
            .SetProperties(SummaryAvatarProperties.Create(character.FightPropMap))
            .SetCritScore(character.FightPropMap)
            .SetLevelNumber(character.PropMap?[PlayerProperty.PROP_LEVEL].Value)
            .SetWeapon(reliquaryAndWeapon.Weapon)
            .SetReliquaries(reliquaryAndWeapon.Reliquaries)
            .SetScore(reliquaryAndWeapon.Reliquaries.Sum(r => r.Score))
            .SetShowcaseRefreshTimeFormat(showcaseRefreshTime, SH.FormatServiceAvatarInfoSummaryShowcaseRefreshTimeFormat, SH.ServiceAvatarInfoSummaryShowcaseNotRefreshed)
            .SetGameRecordRefreshTimeFormat(gameRecordRefreshTime, SH.FormatServiceAvatarInfoSummaryGameRecordRefreshTimeFormat, SH.ServiceAvatarInfoSummaryGameRecordNotRefreshed)
            .SetCalculatorRefreshTimeFormat(calculatorRefreshTime, SH.FormatServiceAvatarInfoSummaryCalculatorRefreshTimeFormat, SH.ServiceAvatarInfoSummaryCalculatorNotRefreshed)
            .SetCostumeIconOrDefault(character, avatar)
            .View;

        return propertyAvatar;
    }

    private ReliquaryAndWeapon ProcessEquip(List<Equip> equipments)
    {
        List<ReliquaryView> reliquaryList = [];
        WeaponView? weapon = null;

        foreach (ref readonly Equip equip in CollectionsMarshal.AsSpan(equipments))
        {
            switch (equip.Flat.ItemType)
            {
                case ItemType.ITEM_RELIQUARY:
                    reliquaryList.Add(SummaryReliquaryFactory.Create(context, character, equip));
                    break;
                case ItemType.ITEM_WEAPON:
                    weapon = CreateWeapon(equip);
                    break;
            }
        }

        return new(reliquaryList, weapon);
    }

    private WeaponView CreateWeapon(DetailedWeapon detailedWeapon)
    {
        MetadataWeapon metadataWeapon = context.IdWeaponMap[detailedWeapon.Id];

        NameValue<string> subProperty;
        if (detailedWeapon.sub is null)
        {
            subProperty = NameValueDefaults.String;
        }
        else
        {
            float statValue = subStat.AppendPropId.GetFormatMethod() is FormatMethod.Percent
                ? subStat.StatValue / 100F
                : subStat.StatValue;
            subProperty = FightPropertyFormat.ToNameValue(subStat.AppendPropId, statValue);
        }

        ArgumentNullException.ThrowIfNull(equip.Weapon);

        return new WeaponViewBuilder()
            .SetName(metadataWeapon.Name)
            .SetIcon(EquipIconConverter.IconNameToUri(metadataWeapon.Icon))
            .SetDescription(metadataWeapon.Description)
            .SetLevel(LevelFormat.Format(equip.Weapon.Level))
            .SetQuality(metadataWeapon.Quality)
            .SetEquipType(EquipType.EQUIP_WEAPON)
            .SetMainProperty(mainStat)
            .SetId(metadataWeapon.Id)
            .SetLevelNumber(equip.Weapon.Level)
            .SetSubProperty(subProperty)
            .SetAffixLevelNumber(detailedWeapon.AffixLevel)
            .SetAffixName(metadataWeapon.Affix?.Name)
            .SetAffixDescription(metadataWeapon.Affix?.Descriptions.Single(a => a.Level == affixLevel).Description)
            .SetWeaponType(metadataWeapon.WeaponType)
            .View;
    }

    private readonly struct ReliquaryAndWeapon
    {
        public readonly List<ReliquaryView> Reliquaries;
        public readonly WeaponView? Weapon;

        public ReliquaryAndWeapon(List<ReliquaryView> reliquaries, WeaponView? weapon)
        {
            Reliquaries = reliquaries;
            Weapon = weapon;
        }
    }
}