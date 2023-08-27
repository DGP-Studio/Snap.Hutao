// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录服务上下文
/// </summary>
internal readonly struct GachaLogServiceMetadataContext
{
    /// <summary>
    /// 物品缓存
    /// </summary>
    public readonly Dictionary<string, Item> ItemCache = new();

    /// <summary>
    /// Id 角色 映射
    /// </summary>
    public readonly Dictionary<AvatarId, Avatar> IdAvatarMap;

    /// <summary>
    /// Id 武器 映射
    /// </summary>
    public readonly Dictionary<WeaponId, Weapon> IdWeaponMap;

    /// <summary>
    /// 名称 角色 映射
    /// </summary>
    public readonly Dictionary<string, Avatar> NameAvatarMap;

    /// <summary>
    /// 名称 武器 映射
    /// </summary>
    public readonly Dictionary<string, Weapon> NameWeaponMap;

    /// <summary>
    /// 是否初始化完成
    /// </summary>
    public readonly bool IsInitialized;

    /// <summary>
    /// 构造一个新的祈愿记录服务上下文
    /// </summary>
    /// <param name="idAvatarMap">Id 角色 映射</param>
    /// <param name="idWeaponMap">Id 武器 映射</param>
    /// <param name="nameAvatarMap">名称 角色 映射</param>
    /// <param name="nameWeaponMap">名称 武器 映射</param>
    public GachaLogServiceMetadataContext(
        Dictionary<AvatarId, Avatar> idAvatarMap,
        Dictionary<WeaponId, Weapon> idWeaponMap,
        Dictionary<string, Avatar> nameAvatarMap,
        Dictionary<string, Weapon> nameWeaponMap)
    {
        IdAvatarMap = idAvatarMap;
        IdWeaponMap = idWeaponMap;
        NameAvatarMap = nameAvatarMap;
        NameWeaponMap = nameWeaponMap;

        IsInitialized = true;
    }

    /// <summary>
    /// 按名称获取物品
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <returns>物品</returns>
    public Item GetItemByNameAndType(string name, string type)
    {
        if (!ItemCache.TryGetValue(name, out Item? result))
        {
            if (type == SH.ModelInterchangeUIGFItemTypeAvatar)
            {
                result = NameAvatarMap[name].ToItem();
            }

            if (type == SH.ModelInterchangeUIGFItemTypeWeapon)
            {
                result = NameWeaponMap[name].ToItem();
            }

            ArgumentNullException.ThrowIfNull(result);
            ItemCache[name] = result;
        }

        return result;
    }

    /// <summary>
    /// 按物品 Id 获取名称星级
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>名称星级</returns>
    public INameQuality GetNameQualityByItemId(uint id)
    {
        uint place = id.StringLength();
        return place switch
        {
            8U => IdAvatarMap[id],
            5U => IdWeaponMap[id],
            _ => throw Must.NeverHappen($"Id places: {place}"),
        };
    }

    /// <summary>
    /// 获取物品 Id
    /// O(1)
    /// </summary>
    /// <param name="item">祈愿物品</param>
    /// <returns>物品 Id</returns>
    public uint GetItemId(GachaLogItem item)
    {
        if (item.ItemType == SH.ModelInterchangeUIGFItemTypeAvatar)
        {
            return NameAvatarMap.GetValueOrDefault(item.Name)?.Id ?? 0;
        }

        if (item.ItemType == SH.ModelInterchangeUIGFItemTypeWeapon)
        {
            return NameWeaponMap.GetValueOrDefault(item.Name)?.Id ?? 0;
        }

        return 0U;
    }
}