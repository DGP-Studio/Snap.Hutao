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
using System.Collections.Immutable;

namespace Snap.Hutao.Service.GachaLog;

internal sealed class GachaLogServiceMetadataContext : IMetadataContext,
    IMetadataSupportInitialization,
    IMetadataArrayGachaEventSource,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource,
    IMetadataDictionaryNameAvatarSource,
    IMetadataDictionaryNameWeaponSource
{
    private readonly Dictionary<string, Item> itemCache = [];

    public ImmutableArray<GachaEvent> GachaEvents { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, Weapon> IdWeaponMap { get; set; } = default!;

    public ImmutableDictionary<string, Avatar> NameAvatarMap { get; set; } = default!;

    public ImmutableDictionary<string, Weapon> NameWeaponMap { get; set; } = default!;

    public bool IsInitialized { get; set; }

    public Item GetItemByNameAndType(string name, string type)
    {
        if (!itemCache.TryGetValue(name, out Item? result))
        {
            if (type == SH.ModelInterchangeUIGFItemTypeAvatar)
            {
                result = NameAvatarMap[name].ToItem<Item>();
            }

            if (type == SH.ModelInterchangeUIGFItemTypeWeapon)
            {
                result = NameWeaponMap[name].ToItem<Item>();
            }

            ArgumentNullException.ThrowIfNull(result);
            itemCache[name] = result;
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