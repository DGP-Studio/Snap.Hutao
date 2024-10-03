// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

internal sealed class GrowCurve
{
    private ImmutableDictionary<GrowCurveType, float>? map;

    public Level Level { get; set; }

    public TypeValueCollection<GrowCurveType, float> Curves { get; set; } = default!;

    public ImmutableDictionary<GrowCurveType, float> Map
    {
        get => map ??= Curves.ToImmutableDictionary<TypeValue<GrowCurveType, float>, GrowCurveType, float>(v => v.Type, v => v.Value);
    }
}