// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Format;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Enka.Model;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;
using ModelAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using PropertyAvatar = Snap.Hutao.ViewModel.AvatarProperty.AvatarView;
using PropertyReliquary = Snap.Hutao.ViewModel.AvatarProperty.ReliquaryView;
using PropertyWeapon = Snap.Hutao.ViewModel.AvatarProperty.WeaponView;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 单个角色工厂
/// </summary>
[HighQuality]
internal sealed class SummaryAvatarFactory
{
    private readonly ModelAvatarInfo avatarInfo;
    private readonly DateTimeOffset showcaseRefreshTime;
    private readonly DateTimeOffset gameRecordRefreshTime;
    private readonly DateTimeOffset calculatorRefreshTime;
    private readonly SummaryMetadataContext metadataContext;

    /// <summary>
    /// 构造一个新的角色工厂
    /// </summary>
    /// <param name="metadataContext">元数据上下文</param>
    /// <param name="avatarInfo">角色信息</param>
    public SummaryAvatarFactory(SummaryMetadataContext metadataContext, EntityAvatarInfo avatarInfo)
    {
        this.metadataContext = metadataContext;
        this.avatarInfo = avatarInfo.Info;

        showcaseRefreshTime = avatarInfo.ShowcaseRefreshTime;
        gameRecordRefreshTime = avatarInfo.GameRecordRefreshTime;
        calculatorRefreshTime = avatarInfo.CalculatorRefreshTime;
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <returns>角色</returns>
    public PropertyAvatar Create()
    {
        ReliquaryAndWeapon reliquaryAndWeapon = ProcessEquip(avatarInfo.EquipList.EmptyIfNull());
        MetadataAvatar avatar = metadataContext.IdAvatarMap[avatarInfo.AvatarId];

        PropertyAvatar propertyAvatar = new()
        {
            // metadata part
            Id = avatar.Id,
            Name = avatar.Name,
            Quality = avatar.Quality,
            NameCard = AvatarNameCardPicConverter.AvatarToUri(avatar),
            Element = ElementNameIconConverter.ElementNameToElementType(avatar.FetterInfo.VisionBefore),

            // webinfo & metadata mixed part
            Constellations = SummaryHelper.CreateConstellations(avatar.SkillDepot.Talents, avatarInfo.TalentIdList),
            Skills = SummaryHelper.CreateSkills(avatarInfo.SkillLevelMap, avatarInfo.ProudSkillExtraLevelMap, avatar.SkillDepot.CompositeSkillsNoInherents()),

            // webinfo part
            FetterLevel = avatarInfo.FetterInfo?.ExpLevel ?? 0U,
            Properties = SummaryAvatarProperties.Create(avatarInfo.FightPropMap),
            CritScore = $"{SummaryHelper.ScoreCrit(avatarInfo.FightPropMap):F2}",
            LevelNumber = avatarInfo.PropMap?[PlayerProperty.PROP_LEVEL].Value ?? 0U,

            // processed webinfo part
            Weapon = reliquaryAndWeapon.Weapon,
            Reliquaries = reliquaryAndWeapon.Reliquaries,
            Score = $"{reliquaryAndWeapon.Reliquaries.Sum(r => r.Score):F2}",

            // times
            ShowcaseRefreshTimeFormat = showcaseRefreshTime == DateTimeOffsetExtension.DatebaseDefaultTime
                ? SH.ServiceAvatarInfoSummaryShowcaseNotRefreshed
                : SH.ServiceAvatarInfoSummaryShowcaseRefreshTimeFormat.Format(showcaseRefreshTime),
            GameRecordRefreshTimeFormat = gameRecordRefreshTime == DateTimeOffsetExtension.DatebaseDefaultTime
                ? SH.ServiceAvatarInfoSummaryGameRecordNotRefreshed
                : SH.ServiceAvatarInfoSummaryGameRecordRefreshTimeFormat.Format(gameRecordRefreshTime),
            CalculatorRefreshTimeFormat = calculatorRefreshTime == DateTimeOffsetExtension.DatebaseDefaultTime
                ? SH.ServiceAvatarInfoSummaryCalculatorNotRefreshed
                : SH.ServiceAvatarInfoSummaryCalculatorRefreshTimeFormat.Format(calculatorRefreshTime),
        };

        ApplyCostumeIconOrDefault(ref propertyAvatar, avatar);
        return propertyAvatar;
    }

    private void ApplyCostumeIconOrDefault(ref PropertyAvatar propertyAvatar, MetadataAvatar avatar)
    {
        if (avatarInfo.CostumeId.TryGetValue(out CostumeId id))
        {
            Model.Metadata.Avatar.Costume costume = avatar.Costumes.Single(c => c.Id == id);

            // Set to costume icon
            propertyAvatar.Icon = AvatarIconConverter.IconNameToUri(costume.FrontIcon);
            propertyAvatar.SideIcon = AvatarIconConverter.IconNameToUri(costume.SideIcon);
        }
        else
        {
            propertyAvatar.Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
            propertyAvatar.SideIcon = AvatarIconConverter.IconNameToUri(avatar.SideIcon);
        }
    }

    private ReliquaryAndWeapon ProcessEquip(List<Web.Enka.Model.Equip> equipments)
    {
        List<PropertyReliquary> reliquaryList = new();
        PropertyWeapon? weapon = null;

        foreach (Web.Enka.Model.Equip equip in equipments)
        {
            switch (equip.Flat.ItemType)
            {
                case ItemType.ITEM_RELIQUARY:
                    SummaryReliquaryFactory summaryReliquaryFactory = new(metadataContext, avatarInfo, equip);
                    reliquaryList.Add(summaryReliquaryFactory.CreateReliquary());
                    break;
                case ItemType.ITEM_WEAPON:
                    weapon = CreateWeapon(equip);
                    break;
            }
        }

        return new(reliquaryList, weapon);
    }

    private PropertyWeapon CreateWeapon(Web.Enka.Model.Equip equip)
    {
        MetadataWeapon weapon = metadataContext.IdWeaponMap[equip.ItemId];

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
            MainProperty = mainStat is not null ? FightPropertyFormat.ToNameValue(mainStat.AppendPropId, mainStat.StatValue) : NameValueDefaults.String,

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