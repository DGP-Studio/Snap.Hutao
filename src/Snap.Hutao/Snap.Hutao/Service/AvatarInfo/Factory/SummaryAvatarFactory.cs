// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.Web.Enka.Model;
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
    private readonly SummaryMetadataContext metadataContext;

    /// <summary>
    /// 构造一个新的角色工厂
    /// </summary>
    /// <param name="metadataContext">元数据上下文</param>
    /// <param name="avatarInfo">角色信息</param>
    public SummaryAvatarFactory(SummaryMetadataContext metadataContext, ModelAvatarInfo avatarInfo)
    {
        this.metadataContext = metadataContext;
        this.avatarInfo = avatarInfo;
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
            FetterLevel = avatarInfo.FetterInfo?.ExpLevel ?? 0,
            Properties = SummaryAvatarProperties.Create(avatarInfo.FightPropMap),
            CritScore = $"{SummaryHelper.ScoreCrit(avatarInfo.FightPropMap):F2}",
            LevelNumber = avatarInfo.PropMap?[PlayerProperty.PROP_LEVEL].Value ?? 0,

            // processed webinfo part
            Weapon = reliquaryAndWeapon.Weapon,
            Reliquaries = reliquaryAndWeapon.Reliquaries,
            Score = $"{reliquaryAndWeapon.Reliquaries.Sum(r => r.Score):F2}",
        };

        TryApplyCostumeIcon(ref propertyAvatar, avatar);
        return propertyAvatar;
    }

    private void TryApplyCostumeIcon(ref PropertyAvatar propertyAvatar, MetadataAvatar avatar)
    {
        if (avatarInfo.CostumeId.HasValue)
        {
            Model.Metadata.Avatar.Costume costume = avatar.Costumes.Single(c => c.Id == avatarInfo.CostumeId.Value);

            // Set to costume icon
            propertyAvatar.Icon = AvatarIconConverter.IconNameToUri(costume.Icon);
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

        // equipments can be null
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

        // AffixMap can be empty when it's a white weapon.
        KeyValuePair<string, int>? idLevel = equip.Weapon!.AffixMap?.Single();
        int affixLevel = idLevel?.Value ?? 0;

        WeaponStat? mainStat = equip.Flat.WeaponStats?.ElementAtOrDefault(0);
        WeaponStat? subStat = equip.Flat.WeaponStats?.ElementAtOrDefault(1);

        NameDescription subProperty;
        if (subStat == null)
        {
            subProperty = new(string.Empty, string.Empty);
        }
        else
        {
            // 是否为整数
            subStat.StatValue = subStat.StatValue == MathF.Truncate(subStat.StatValue)
                ? subStat.StatValue / 100F
                : subStat.StatValue;
            subProperty = FightPropertyFormat.ToNameDescription(subStat.AppendPropId, subStat.StatValue);
        }

        return new()
        {
            // NameIconDescription
            Name = weapon.Name,
            Icon = EquipIconConverter.IconNameToUri(weapon.Icon),
            Description = weapon.Description,

            // EquipBase
            Level = $"Lv.{equip.Weapon!.Level}",
            Quality = weapon.Quality,
            MainProperty = mainStat != null ? FightPropertyFormat.ToNameValue(mainStat.AppendPropId, mainStat.StatValue) : default!,

            // Weapon
            Id = weapon.Id,
            LevelNumber = equip.Weapon!.Level,
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