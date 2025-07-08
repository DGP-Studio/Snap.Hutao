// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.Wiki;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

internal static class TypeValueCollectionExtension
{
    public static ImmutableArray<PropertyCurveValue> ToPropertyCurveValues(this TypeValueCollection<FightProperty, GrowCurveType> collection, BaseValue baseValue)
    {
        return collection.Array.SelectAsArray(static (info, baseValue) => new PropertyCurveValue(info.Type, info.Value, baseValue), baseValue);
    }
}