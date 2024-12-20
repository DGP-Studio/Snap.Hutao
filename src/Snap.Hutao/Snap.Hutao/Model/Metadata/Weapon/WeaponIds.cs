// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Frozen;

namespace Snap.Hutao.Model.Metadata.Weapon;

internal static class WeaponIds
{
    public static readonly FrozenSet<uint> BlueStandardWeaponIds =
    [
        11301U, 11302U, 11306U,
        12301U, 12302U, 12305U,
        13303U,
        14301U, 14302U, 14304U,
        15301U, 15302U, 15304U
    ];

    public static readonly FrozenSet<uint> PurpleStandardWeaponIds =
    [
        11401U, 11402U, 11403U, 11405U,
        12401U, 12402U, 12403U, 12405U,
        13401U, 13407U,
        14401U, 14402U, 14403U, 14409U,
        15401U, 15402U, 15403U, 15405U
    ];

    public static readonly FrozenSet<WeaponId> OrangeStandardWishIds =
    [
        11501U, 11502U,
        12501U, 12502U,
        13502U, 13505U,
        14501U, 14502U,
        15501U, 15502U,
    ];

    public static bool IsOrangeStandardWish(in WeaponId weaponId)
    {
        return OrangeStandardWishIds.Contains(weaponId);
    }
}