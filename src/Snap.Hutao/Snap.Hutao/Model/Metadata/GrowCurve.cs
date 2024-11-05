// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

internal sealed class GrowCurve
{
    public Level Level { get; set; }

    public TypeValueCollection<GrowCurveType, float> Curves { get; set; } = default!;
}