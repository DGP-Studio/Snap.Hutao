// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.Wiki;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

internal static class TypeValueCollectionExtension
{
    public static ImmutableArray<PropertyCurveValue> GetPropertyCurveValues(this TypeValueCollection<FightProperty, GrowCurveType> collection, BaseValue baseValue)
    {
        return collection.TypeValues.Select(info => new PropertyCurveValue(info.Key, info.Value, baseValue.GetValue(info.Key))).ToImmutableArray();
    }
}
