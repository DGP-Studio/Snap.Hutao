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

    public List<TypeValue<GrowCurveType, float>> Curves { get; set; } = default!;

    public ImmutableDictionary<GrowCurveType, float> Map
    {
        get => map ??= Curves.ToImmutableDictionary(v => v.Type, v => v.Value);
    }
}