// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Wiki;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Converter;

internal static class BaseValueInfoFormat
{
    public static NameValue<string> ToNameValue(PropertyCurveValue propValue, Level level, PromoteLevel promoteLevel, ImmutableDictionary<Level, ImmutableDictionary<GrowCurveType, float>> growCurveMap, IReadOnlyDictionary<PromoteLevel, Promote>? promoteMap)
    {
        float value = propValue.Value * growCurveMap[level].GetValueOrDefault(propValue.Type);
        if (promoteMap is not null)
        {
            value += promoteMap[promoteLevel].GetValue(propValue.Property);
        }

        return FightPropertyFormat.ToNameValue(propValue.Property, value);
    }
}