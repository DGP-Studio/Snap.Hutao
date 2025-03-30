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
    IMetadataArrayGachaEventSource,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource,
    IMetadataDictionaryNameAvatarSource,
    IMetadataDictionaryNameWeaponSource
{
    public ImmutableArray<GachaEvent> GachaEvents { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, Weapon> IdWeaponMap { get; set; } = default!;

    public ImmutableDictionary<string, Avatar> NameAvatarMap { get; set; } = default!;

    public ImmutableDictionary<string, Weapon> NameWeaponMap { get; set; } = default!;

    public Item GetItemByNameAndType(string name, string type)
    {
        if (string.Equals(type, SH.ModelInterchangeUIGFItemTypeAvatar, StringComparison.Ordinal))
        {
            return this.GetAvatar(name).GetOrCreateItem();
        }

        if (string.Equals(type, SH.ModelInterchangeUIGFItemTypeWeapon, StringComparison.Ordinal))
        {
            return this.GetWeapon(name).GetOrCreateItem();
        }

        throw HutaoException.NotSupported($"Unsupported item type and name: '{type},{name}'");
    }

    public uint GetItemId(GachaLogItem item)
    {
        if (string.Equals(item.ItemType, SH.ModelInterchangeUIGFItemTypeAvatar, StringComparison.Ordinal))
        {
            return this.GetAvatar(item.Name).Id;
        }

        if (string.Equals(item.ItemType, SH.ModelInterchangeUIGFItemTypeWeapon, StringComparison.Ordinal))
        {
            return this.GetWeapon(item.Name).Id;
        }

        throw HutaoException.NotSupported($"Unsupported item type and name: '{item.ItemType},{item.Name}'");
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
}