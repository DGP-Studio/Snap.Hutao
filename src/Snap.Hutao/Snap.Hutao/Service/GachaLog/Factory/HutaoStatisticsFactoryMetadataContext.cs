// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal readonly struct HutaoStatisticsFactoryMetadataContext
{
    public readonly Dictionary<AvatarId, Avatar> IdAvatarMap;
    public readonly Dictionary<WeaponId, Weapon> IdWeaponMap;
    public readonly List<GachaEvent> GachaEvents;

    public HutaoStatisticsFactoryMetadataContext(Dictionary<AvatarId, Avatar> idAvatarMap, Dictionary<WeaponId, Weapon> idWeaponMap, List<GachaEvent> gachaEvents)
    {
        IdAvatarMap = idAvatarMap;
        IdWeaponMap = idWeaponMap;
        GachaEvents = gachaEvents;
    }
}