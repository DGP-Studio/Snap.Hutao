// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Reliquary;

internal sealed class ReliquarySubAffix : IDefaultIdentity<ReliquarySubAffixId>
{
    public required ReliquarySubAffixId Id { get; init; }

    public required FightProperty Type { get; init; }

    public required float Value { get; init; }
}