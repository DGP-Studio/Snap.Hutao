// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Weapon;

namespace Snap.Hutao.ViewModel.Wiki;

internal sealed class PropertyCurveValue
{
    public PropertyCurveValue(FightProperty property, GrowCurveType type, float value)
    {
        Property = property;
        Type = type;
        Value = value;
    }

    public PropertyCurveValue(FightProperty property, TypeValueCollection<FightProperty, GrowCurveType> growCurve, BaseValue baseValue)
        : this(property, growCurve.GetValueOrDefault(property), baseValue.GetValue(property))
    {
    }

    public FightProperty Property { get; }

    public GrowCurveType Type { get; }

    public float Value { get; }

    public static PropertyCurveValue From(WeaponTypeValue curveInfo)
    {
        return new(curveInfo.Type, curveInfo.Value, curveInfo.InitValue);
    }
}
