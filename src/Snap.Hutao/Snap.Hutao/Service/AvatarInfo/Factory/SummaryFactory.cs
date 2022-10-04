// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Enka.Model;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;
using ModelAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using ModelPlayerInfo = Snap.Hutao.Web.Enka.Model.PlayerInfo;
using PropertyAvatar = Snap.Hutao.Model.Binding.AvatarProperty.Avatar;
using PropertyReliquary = Snap.Hutao.Model.Binding.AvatarProperty.Reliquary;
using PropertyWeapon = Snap.Hutao.Model.Binding.AvatarProperty.Weapon;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 简述工厂
/// </summary>
[Injection(InjectAs.Transient, typeof(ISummaryFactory))]
internal class SummaryFactory : ISummaryFactory
{
    private readonly IMetadataService metadataService;

    /// <summary>
    /// 构造一个新的简述工厂
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    public SummaryFactory(IMetadataService metadataService)
    {
        this.metadataService = metadataService;
    }

    /// <inheritdoc/>
    public async Task<Summary> CreateAsync(ModelPlayerInfo playerInfo, IEnumerable<ModelAvatarInfo> avatarInfos)
    {
        Dictionary<int, MetadataAvatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
        Dictionary<int, FightProperty> idRelicMainPropMap = await metadataService.GetIdToReliquaryMainPropertyMapAsync().ConfigureAwait(false);
        Dictionary<int, MetadataWeapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);
        Dictionary<int, ReliquaryAffix> idReliquaryAffixMap = await metadataService.GetIdReliquaryAffixMapAsync().ConfigureAwait(false);

        List<ReliquaryLevel> reliqueryLevels = await metadataService.GetReliquaryLevelsAsync().ConfigureAwait(false);
        List<MetadataReliquary> reliquaries = await metadataService.GetReliquariesAsync().ConfigureAwait(false);

        SummaryFactoryInner inner = new(idAvatarMap, idRelicMainPropMap, idWeaponMap, idReliquaryAffixMap, reliqueryLevels, reliquaries);
        return inner.Create(playerInfo, avatarInfos);
    }

    private class SummaryFactoryInner
    {
        private readonly Dictionary<int, MetadataAvatar> idAvatarMap;
        private readonly Dictionary<int, FightProperty> idRelicMainPropMap;
        private readonly Dictionary<int, MetadataWeapon> idWeaponMap;
        private readonly Dictionary<int, ReliquaryAffix> idReliquaryAffixMap;
        private readonly List<ReliquaryLevel> reliqueryLevels;
        private readonly List<MetadataReliquary> reliquaries;

        public SummaryFactoryInner(
            Dictionary<int, MetadataAvatar> idAvatarMap,
            Dictionary<int, FightProperty> idRelicMainPropMap,
            Dictionary<int, MetadataWeapon> idWeaponMap,
            Dictionary<int, ReliquaryAffix> idReliquaryAffixMap,
            List<ReliquaryLevel> reliqueryLevels,
            List<MetadataReliquary> reliquaries)
        {
            this.idAvatarMap = idAvatarMap;
            this.idRelicMainPropMap = idRelicMainPropMap;
            this.idWeaponMap = idWeaponMap;
            this.reliqueryLevels = reliqueryLevels;
            this.reliquaries = reliquaries;
            this.idReliquaryAffixMap = idReliquaryAffixMap;
        }

        public Summary Create(ModelPlayerInfo playerInfo, IEnumerable<ModelAvatarInfo> avatarInfos)
        {
            // 用作头像
            MetadataAvatar avatar = idAvatarMap[playerInfo.ProfilePicture.AvatarId];

            return new()
            {
                Player = SummaryHelper.CreatePlayer(playerInfo, avatar),
                Avatars = avatarInfos.Select(a => CreateAvatar(a)).ToList(),
            };
        }

        private PropertyAvatar CreateAvatar(ModelAvatarInfo avatarInfo)
        {
            (List<PropertyReliquary> reliquaries, PropertyWeapon weapon) = ProcessEquip(avatarInfo.EquipList);
            MetadataAvatar avatar = idAvatarMap[avatarInfo.AvatarId];

            return new()
            {
                Name = avatar.Name,
                Icon = AvatarIconConverter.IconNameToUri(avatar.Icon),
                SideIcon = AvatarIconConverter.IconNameToUri(avatar.SideIcon),
                Quality = avatar.Quality,
                Level = avatarInfo.PropMap[PlayerProperty.PROP_LEVEL].Value ?? string.Empty,
                FetterLevel = avatarInfo.FetterInfo.ExpLevel,
                Weapon = weapon,
                Reliquaries = reliquaries,
                Constellations = SummaryHelper.CreateConstellations(avatarInfo.TalentIdList, avatar.SkillDepot.Talents),
                Skills = SummaryHelper.CreateSkills(avatarInfo.SkillLevelMap, avatarInfo.ProudSkillExtraLevelMap, avatar.SkillDepot.GetCompositeSkillsNoInherents()),
                Properties = SummaryHelper.CreateAvatarProperties(avatarInfo.FightPropMap),
            };
        }

        private (List<PropertyReliquary> Reliquaries, PropertyWeapon Weapon) ProcessEquip(IList<Equip> equipments)
        {
            List<PropertyReliquary> reliquaries = new();
            PropertyWeapon? weapon = null;

            foreach (Equip equip in equipments)
            {
                switch (equip.Flat.ItemType)
                {
                    case ItemType.ITEM_RELIQUARY:
                        reliquaries.Add(CreateReliquary(equip));
                        break;
                    case ItemType.ITEM_WEAPON:
                        weapon = CreateWeapon(equip);
                        break;
                }
            }

            return (reliquaries, weapon!);
        }

        private PropertyReliquary CreateReliquary(Equip equip)
        {
            MetadataReliquary reliquary = reliquaries.Single(r => r.Ids.Contains(equip.ItemId));

            return new()
            {
                // NameIconDescription
                Name = reliquary.Name,
                Icon = RelicIconConverter.IconNameToUri(reliquary.Icon),
                Description = reliquary.Description,

                // EquipBase
                Level = $"+{equip.Reliquary!.Level - 1}",
                Quality = reliquary.RankLevel,
                MainProperty = CreateReliquaryMainProperty(equip.Reliquary.MainPropId, reliquary.RankLevel, equip.Reliquary.Level),

                // Reliquary
                SubProperties = equip.Reliquary.AppendPropIdList.Select(id => CreateReliquarySubProperty(id)).ToList(),
            };
        }

        private Pair<string, string> CreateReliquaryMainProperty(int propId, ItemQuality quality, int level)
        {
            ReliquaryLevel reliquaryLevel = reliqueryLevels.Single(r => r.Level == level && r.Quality == quality);
            FightProperty property = idRelicMainPropMap[propId];

            return new(property.GetDescription(), PropertyInfoDescriptor.FormatProperty(property, reliquaryLevel.Properties[property]));
        }

        private Pair<string, string> CreateReliquarySubProperty(int appendPropId)
        {
            ReliquaryAffix affix = idReliquaryAffixMap[appendPropId];
            FightProperty property = affix.Type;

            return new(property.GetDescription(), PropertyInfoDescriptor.FormatProperty(property, affix.Value));
        }

        private PropertyWeapon CreateWeapon(Equip equip)
        {
            MetadataWeapon weapon = idWeaponMap[equip.ItemId];
            (string id, int level) = equip.Weapon!.AffixMap.Single();

            return new()
            {
                // NameIconDescription
                Name = weapon.Name,
                Icon = EquipIconConverter.IconNameToUri(weapon.Icon),
                Description = weapon.Description,

                // EquipBase
                Level = $"Lv.{equip.Weapon!.Level}",
                Quality = weapon.Quality,
                MainProperty = new(string.Empty, string.Empty), // TODO

                // Weapon
                SubProperty = new(string.Empty, string.Empty), // TODO
                AffixLevel = $"精炼{level + 1}",
                AffixName = weapon.Affix?.Name ?? string.Empty,
                AffixDescription = weapon.Affix?.Descriptions.Single(a => a.Level == level).Description ?? string.Empty,
            };
        }
    }
}