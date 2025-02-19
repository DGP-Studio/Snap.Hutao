// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.Wiki;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Weapon;

internal static class WeaponTypeValueCollectionExtension
{
    public static ImmutableArray<PropertyCurveValue> ToPropertyCurveValues(this WeaponTypeValueCollection collection)
    {
        return [.. collection.Array.Select(curve => new PropertyCurveValue(curve.Type, curve.Value, curve.InitValue))];
    }
}