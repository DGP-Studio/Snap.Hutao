// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal sealed class GachaStatisticsItemCounter
{
    public GachaStatisticsItemCounter(GachaStatisticsFactoryContext context)
    {
        if (!context.IsUnobtainedWishItemVisible)
        {
            OrangeAvatar = [];
            PurpleAvatar = [];
            OrangeWeapon = [];
            PurpleWeapon = [];
            BlueWeapon = [];
            return;
        }

        OrangeAvatar = context.Metadata.IdAvatarMap.Values
            .Where(avatar => avatar.Quality == QualityType.QUALITY_ORANGE)
            .ToDictionary(avatar => avatar, _ => 0);
        PurpleAvatar = context.Metadata.IdAvatarMap.Values
            .Where(avatar => avatar.Quality == QualityType.QUALITY_PURPLE)
            .ToDictionary(avatar => avatar, _ => 0);
        OrangeWeapon = context.Metadata.IdWeaponMap.Values
            .Where(weapon => weapon.Quality == QualityType.QUALITY_ORANGE)
            .ToDictionary(weapon => weapon, _ => 0);

        HashSet<Weapon> purpleWeapons = [];
        foreach (uint weaponId in WeaponIds.PurpleStandardWeaponIds)
        {
            purpleWeapons.Add(context.Metadata.GetWeapon(weaponId));
        }

        foreach (ref readonly GachaEvent gachaEvent in context.Metadata.GachaEvents.AsSpan())
        {
            if (gachaEvent.Type is GachaType.ActivityWeapon)
            {
                foreach (uint weaponId in gachaEvent.UpPurpleList)
                {
                    purpleWeapons.Add(context.Metadata.GetWeapon(weaponId));
                }
            }
        }

        HashSet<Weapon> blueWeapons = [];
        foreach (uint weaponId in WeaponIds.BlueStandardWeaponIds)
        {
            blueWeapons.Add(context.Metadata.GetWeapon(weaponId));
        }

        PurpleWeapon = purpleWeapons.ToDictionary(weapon => weapon, _ => 0);
        BlueWeapon = blueWeapons.ToDictionary(weapon => weapon, _ => 0);
    }

    public Dictionary<Avatar, int> OrangeAvatar { get; }

    public Dictionary<Avatar, int> PurpleAvatar { get; }

    public Dictionary<Weapon, int> OrangeWeapon { get; }

    public Dictionary<Weapon, int> PurpleWeapon { get; }

    public Dictionary<Weapon, int> BlueWeapon { get; }
}