// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Web.Enka.Model;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
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
    private readonly Dictionary<int, MetadataAvatar> idAvatarMap;
    private readonly Dictionary<int, FightProperty> idRelicMainPropMap;
    private readonly Dictionary<int, ReliquaryAffix> idReliquaryAffixMap;
    private readonly Dictionary<int, MetadataWeapon> idWeaponMap;
    private readonly List<ReliquaryLevel> reliqueryLevels;
    private readonly List<MetadataReliquary> reliquaries;

    private readonly ModelAvatarInfo avatarInfo;

    /// <summary>
    /// 构造一个新的角色工厂
    /// </summary>
    /// <param name="idAvatarMap">角色映射</param>
    /// <param name="idWeaponMap">武器映射</param>
    /// <param name="idRelicMainPropMap">圣遗物主属性映射</param>
    /// <param name="idReliquaryAffixMap">圣遗物副词条映射</param>
    /// <param name="reliqueryLevels">圣遗物主属性等级</param>
    /// <param name="reliquaries">圣遗物</param>
    /// <param name="avatarInfo">角色信息</param>
    public SummaryAvatarFactory(
        Dictionary<int, MetadataAvatar> idAvatarMap,
        Dictionary<int, MetadataWeapon> idWeaponMap,
        Dictionary<int, FightProperty> idRelicMainPropMap,
        Dictionary<int, ReliquaryAffix> idReliquaryAffixMap,
        List<ReliquaryLevel> reliqueryLevels,
        List<MetadataReliquary> reliquaries,
        ModelAvatarInfo avatarInfo)
    {
        this.idAvatarMap = idAvatarMap;
        this.idRelicMainPropMap = idRelicMainPropMap;
        this.idReliquaryAffixMap = idReliquaryAffixMap;
        this.idWeaponMap = idWeaponMap;
        this.reliqueryLevels = reliqueryLevels;
        this.reliquaries = reliquaries;
        this.avatarInfo = avatarInfo;
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <returns>角色</returns>
    public PropertyAvatar CreateAvatar()
    {
        ReliquaryAndWeapon reliquaryAndWeapon = ProcessEquip(avatarInfo.EquipList);
        MetadataAvatar avatar = idAvatarMap[avatarInfo.AvatarId];

        return new()
        {
            Name = avatar.Name,
            Icon = AvatarIconConverter.IconNameToUri(avatar.Icon),
            SideIcon = AvatarIconConverter.IconNameToUri(avatar.SideIcon),
            Quality = avatar.Quality,
            Level = $"Lv.{avatarInfo.PropMap[PlayerProperty.PROP_LEVEL].Value}",
            FetterLevel = avatarInfo.FetterInfo.ExpLevel,
            Weapon = reliquaryAndWeapon.Weapon,
            Reliquaries = reliquaryAndWeapon.Reliquaries,
            Constellations = SummaryHelper.CreateConstellations(avatarInfo.TalentIdList, avatar.SkillDepot.Talents),
            Skills = SummaryHelper.CreateSkills(avatarInfo.SkillLevelMap, avatarInfo.ProudSkillExtraLevelMap, avatar.SkillDepot.GetCompositeSkillsNoInherents()),
            Properties = SummaryHelper.CreateAvatarProperties(avatarInfo.FightPropMap),
            Score = reliquaryAndWeapon.Reliquaries.Sum(r => r.Score).ToString("F2"),
            CritScore = $"{SummaryHelper.ScoreCrit(avatarInfo.FightPropMap):F2}",
        };
    }

    private ReliquaryAndWeapon ProcessEquip(IList<Equip> equipments)
    {
        List<PropertyReliquary> reliquaryList = new();
        PropertyWeapon? weapon = null;

        foreach (Equip equip in equipments)
        {
            switch (equip.Flat.ItemType)
            {
                case ItemType.ITEM_RELIQUARY:
                    SummaryReliquaryFactory summaryReliquaryFactory = new(idReliquaryAffixMap, idRelicMainPropMap, reliqueryLevels, reliquaries, avatarInfo, equip);
                    reliquaryList.Add(summaryReliquaryFactory.CreateReliquary());
                    break;
                case ItemType.ITEM_WEAPON:
                    weapon = CreateWeapon(equip);
                    break;
            }
        }

        return new(reliquaryList, weapon!);
    }

    private PropertyWeapon CreateWeapon(Equip equip)
    {
        MetadataWeapon weapon = idWeaponMap[equip.ItemId];

        // AffixMap can be empty when it's a white weapon.
        KeyValuePair<string, int>? idLevel = equip.Weapon!.AffixMap?.Single();
        int affixLevel = idLevel.HasValue ? idLevel.Value.Value : 0;

        WeaponStat mainStat = equip.Flat.WeaponStats![0];
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
            MainProperty = new(mainStat.AppendPropId.GetDescription(), mainStat.StatValue.ToString()),

            // Weapon
            SubProperty = subProperty,
            AffixLevel = $"精炼{affixLevel + 1}",
            AffixName = weapon.Affix?.Name ?? string.Empty,
            AffixDescription = weapon.Affix?.Descriptions.Single(a => a.Level == affixLevel).Description ?? string.Empty,
        };
    }

    private struct ReliquaryAndWeapon
    {
        public List<PropertyReliquary> Reliquaries;
        public PropertyWeapon Weapon;

        public ReliquaryAndWeapon(List<PropertyReliquary> reliquaries, PropertyWeapon weapon)
        {
            Reliquaries = reliquaries;
            Weapon = weapon;
        }
    }
}
