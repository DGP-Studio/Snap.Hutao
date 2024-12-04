// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Reliquary;

internal class ReliquaryMainAffix
{
    public required ReliquaryMainAffixId Id { get; init; }

    public required FightProperty Type { get; init; }
}