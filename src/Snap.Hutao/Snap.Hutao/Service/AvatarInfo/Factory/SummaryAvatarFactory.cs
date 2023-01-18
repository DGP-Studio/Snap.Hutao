// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Web.Enka.Model;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;
using ModelAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using PropertyAvatar = Snap.Hutao.Model.Binding.AvatarProperty.Avatar;
using PropertyReliquary = Snap.Hutao.Model.Binding.AvatarProperty.Reliquary;
using PropertyWeapon = Snap.Hutao.Model.Binding.AvatarProperty.Weapon;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 简述角色工厂
/// </summary>
internal class SummaryAvatarFactory
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
    public PropertyAvatar CreateAvatar()
    {
        ReliquaryAndWeapon reliquaryAndWeapon = ProcessEquip(avatarInfo.EquipList.EmptyIfNull());
        MetadataAvatar avatar = metadataContext.IdAvatarMap[avatarInfo.AvatarId];

        PropertyAvatar propertyAvatar = new()
        {
            // metadata part
            Id = avatar.Id,
            Name = avatar.Name,
            NameCard = AvatarNameCardPicConverter.AvatarToUri(avatar),
            Quality = avatar.Quality,
            Element = ElementNameIconConverter.ElementNameToElementType(avatar.FetterInfo.VisionBefore),

            // webinfo & metadata mixed part
            Constellations = SummaryHelper.CreateConstellations(avatarInfo.TalentIdList, avatar.SkillDepot.Talents),
            Skills = SummaryHelper.CreateSkills(avatarInfo.SkillLevelMap, avatarInfo.ProudSkillExtraLevelMap, avatar.SkillDepot.GetCompositeSkillsNoInherents()),

            // webinfo part
            FetterLevel = avatarInfo.FetterInfo?.ExpLevel ?? 0,
            Properties = SummaryHelper.CreateAvatarProperties(avatarInfo.FightPropMap),
            CritScore = $"{SummaryHelper.ScoreCrit(avatarInfo.FightPropMap):F2}",
            LevelNumber = int.Parse(avatarInfo.PropMap?[PlayerProperty.PROP_LEVEL].Value ?? string.Empty),

            // processed webinfo part
            Weapon = reliquaryAndWeapon.Weapon,
            Reliquaries = reliquaryAndWeapon.Reliquaries,
            Score = reliquaryAndWeapon.Reliquaries.Sum(r => r.Score).ToString("F2"),
        };

        TryApplyCostumeIconToAvatar(ref propertyAvatar, avatar);

        propertyAvatar.Level = $"Lv.{propertyAvatar.LevelNumber}";
        return propertyAvatar;
    }

    private void TryApplyCostumeIconToAvatar(ref PropertyAvatar propertyAvatar, MetadataAvatar avatar)
    {
        // Set to costume icon
        if (avatarInfo.CostumeId.HasValue)
        {
            int costumeId = avatarInfo.CostumeId.Value;
            Model.Metadata.Avatar.Costume costume = avatar.Costumes.Single(c => c.Id == costumeId);

            propertyAvatar.Icon = AvatarIconConverter.IconNameToUri(costume.Icon);
            propertyAvatar.SideIcon = AvatarIconConverter.IconNameToUri(costume.SideIcon);
        }
        else
        {
            propertyAvatar.Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
            propertyAvatar.SideIcon = AvatarIconConverter.IconNameToUri(avatar.SideIcon);
        }
    }

    private ReliquaryAndWeapon ProcessEquip(List<Equip> equipments)
    {
        List<PropertyReliquary> reliquaryList = new();
        PropertyWeapon? weapon = null;

        // equipments can be null
        foreach (Equip equip in equipments)
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

    private PropertyWeapon CreateWeapon(Equip equip)
    {
        MetadataWeapon weapon = metadataContext.IdWeaponMap[equip.ItemId];

        // AffixMap can be empty when it's a white weapon.
        KeyValuePair<string, int>? idLevel = equip.Weapon!.AffixMap?.Single();
        int affixLevel = idLevel.HasValue ? idLevel.Value.Value : 0;

        WeaponStat? mainStat = equip.Flat.WeaponStats?[0];
        WeaponStat? subStat = equip.Flat.WeaponStats?.Count > 1 ? equip.Flat.WeaponStats![1] : null;

        Pair<string, string> subProperty;
        if (subStat == null)
        {
            subProperty = new(string.Empty, string.Empty);
        }
        else
        {
            subStat.StatValue = subStat.StatValue - Math.Truncate(subStat.StatValue) > 0 ? subStat.StatValue / 100D : subStat.StatValue;
            subProperty = PropertyInfoDescriptor.FormatPair(subStat.AppendPropId, subStat.StatValue);
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
            MainProperty = mainStat == null ? default! : new(mainStat.AppendPropId.GetDescription(), mainStat.StatValue.ToString()),

            // Weapon
            Id = weapon.Id,
            LevelNumber = equip.Weapon!.Level,
            SubProperty = subProperty,
            AffixLevel = $"精炼{affixLevel + 1}",
            AffixName = weapon.Affix?.Name ?? string.Empty,
            AffixDescription = weapon.Affix?.Descriptions.Single(a => a.Level == affixLevel).Description ?? string.Empty,
        };
    }

    private struct ReliquaryAndWeapon
    {
        public List<PropertyReliquary> Reliquaries;
        public PropertyWeapon? Weapon;

        public ReliquaryAndWeapon(List<PropertyReliquary> reliquaries, PropertyWeapon? weapon)
        {
            Reliquaries = reliquaries;
            Weapon = weapon;
        }
    }
}
