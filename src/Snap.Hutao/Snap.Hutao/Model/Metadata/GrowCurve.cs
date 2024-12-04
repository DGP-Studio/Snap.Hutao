// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

internal sealed class GrowCurve
{
    public required Level Level { get; init; }

    public required TypeValueCollection<GrowCurveType, float> Curves { get; init; }
}