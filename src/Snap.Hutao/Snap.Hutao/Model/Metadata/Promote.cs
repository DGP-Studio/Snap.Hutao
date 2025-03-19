// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

internal sealed class Promote
{
    public required PromoteId Id { get; init; }

    public required PromoteLevel Level { get; init; }

    public required TypeValueCollection<FightProperty, float> AddProperties { get; init; }
}