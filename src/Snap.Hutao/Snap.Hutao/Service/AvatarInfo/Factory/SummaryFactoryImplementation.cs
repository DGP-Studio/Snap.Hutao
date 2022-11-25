// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Primitive;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;
using ModelAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using ModelPlayerInfo = Snap.Hutao.Web.Enka.Model.PlayerInfo;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 真正实现
/// </summary>
internal class SummaryFactoryImplementation
{
    private readonly Dictionary<AvatarId, MetadataAvatar> idAvatarMap;
    private readonly Dictionary<WeaponId, MetadataWeapon> idWeaponMap;
    private readonly Dictionary<ReliquaryMainAffixId, FightProperty> idRelicMainPropMap;
    private readonly Dictionary<ReliquaryAffixId, ReliquaryAffix> idReliquaryAffixMap;
    private readonly List<ReliquaryLevel> reliqueryLevels;
    private readonly List<MetadataReliquary> reliquaries;

    /// <summary>
    /// 装配一个工厂实现
    /// </summary>
    /// <param name="idAvatarMap">角色映射</param>
    /// <param name="idWeaponMap">武器映射</param>
    /// <param name="idRelicMainPropMap">圣遗物主属性映射</param>
    /// <param name="idReliquaryAffixMap">圣遗物副词条映射</param>
    /// <param name="reliqueryLevels">圣遗物主属性等级</param>
    /// <param name="reliquaries">圣遗物</param>
    public SummaryFactoryImplementation(
        Dictionary<AvatarId, MetadataAvatar> idAvatarMap,
        Dictionary<WeaponId, MetadataWeapon> idWeaponMap,
        Dictionary<ReliquaryMainAffixId, FightProperty> idRelicMainPropMap,
        Dictionary<ReliquaryAffixId, ReliquaryAffix> idReliquaryAffixMap,
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

    /// <summary>
    /// 创建一个新的属性统计对象
    /// </summary>
    /// <param name="playerInfo">玩家信息</param>
    /// <param name="avatarInfos">角色信息</param>
    /// <returns>属性统计</returns>
    public Summary Create(ModelPlayerInfo playerInfo, IEnumerable<ModelAvatarInfo> avatarInfos)
    {
        return new()
        {
            Player = SummaryHelper.CreatePlayer(playerInfo),
            Avatars = avatarInfos.Where(a => !AvatarIds.IsPlayer(a.AvatarId)).Select(a =>
            {
                SummaryAvatarFactory summaryAvatarFactory = new(
                    idAvatarMap,
                    idWeaponMap,
                    idRelicMainPropMap,
                    idReliquaryAffixMap,
                    reliqueryLevels,
                    reliquaries,
                    a);
                return summaryAvatarFactory.CreateAvatar();
            }).ToList(),
        };
    }
}
