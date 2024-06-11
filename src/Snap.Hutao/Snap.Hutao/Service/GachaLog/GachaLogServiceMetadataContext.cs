// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录服务上下文
/// </summary>
internal sealed class GachaLogServiceMetadataContext : IMetadataContext,
    IMetadataSupportInitialization,
    IMetadataListGachaEventSource,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource,
    IMetadataDictionaryNameAvatarSource,
    IMetadataDictionaryNameWeaponSource
{
    public Dictionary<string, Item> ItemCache { get; set; } = [];

    public List<GachaEvent> GachaEvents { get; set; } = default!;

    public Dictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;

    public Dictionary<WeaponId, Weapon> IdWeaponMap { get; set; } = default!;

    public Dictionary<string, Avatar> NameAvatarMap { get; set; } = default!;

    public Dictionary<string, Weapon> NameWeaponMap { get; set; } = default!;

    public bool IsInitialized { get; set; }

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

    public INameQualityAccess GetNameQualityByItemId(uint id)
    {
        uint place = id.StringLength();
        return place switch
        {
            8U => IdAvatarMap[id],
            5U => IdWeaponMap[id],
            _ => throw HutaoException.NotSupported($"Id places: {place}"),
        };
    }

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