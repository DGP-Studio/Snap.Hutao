// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Weapon;

internal sealed class WeaponTypeValue : TypeValue<FightProperty, GrowCurveType>
{
    public WeaponTypeValue(FightProperty type, GrowCurveType value, float initValue)
        : base(type, value)
    {
        InitValue = initValue;
    }

    public float InitValue { get; }
}