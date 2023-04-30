// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录服务上下文
/// </summary>
internal readonly struct GachaLogServiceContext
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
    /// 存档集合
    /// </summary>
    public readonly ObservableCollection<GachaArchive> ArchiveCollection;

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
    /// <param name="archiveCollection">存档集合</param>
    public GachaLogServiceContext(
        Dictionary<AvatarId, Avatar> idAvatarMap,
        Dictionary<WeaponId, Weapon> idWeaponMap,
        Dictionary<string, Avatar> nameAvatarMap,
        Dictionary<string, Weapon> nameWeaponMap,
        ObservableCollection<GachaArchive> archiveCollection)
    {
        IdAvatarMap = idAvatarMap;
        IdWeaponMap = idWeaponMap;
        NameAvatarMap = nameAvatarMap;
        NameWeaponMap = nameWeaponMap;
        ArchiveCollection = archiveCollection;

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
            result = type switch
            {
                "角色" => NameAvatarMap[name].ToItemBase(),
                "武器" => NameWeaponMap[name].ToItemBase(),
                _ => throw Must.NeverHappen(),
            };

            ItemCache[name] = result;
        }

        return result;
    }

    /// <summary>
    /// 按物品 Id 获取名称星级
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>名称星级</returns>
    public INameQuality GetNameQualityByItemId(int id)
    {
        int place = id.Place();
        return place switch
        {
            8 => IdAvatarMap![id],
            5 => IdWeaponMap![id],
            _ => throw Must.NeverHappen($"Id places: {place}"),
        };
    }

    /// <summary>
    /// 获取物品 Id
    /// O(1)
    /// </summary>
    /// <param name="item">祈愿物品</param>
    /// <returns>物品 Id</returns>
    public int GetItemId(GachaLogItem item)
    {
        return item.ItemType switch
        {
            "角色" => NameAvatarMap!.GetValueOrDefault(item.Name)?.Id ?? 0,
            "武器" => NameWeaponMap!.GetValueOrDefault(item.Name)?.Id ?? 0,
            _ => 0,
        };
    }
}