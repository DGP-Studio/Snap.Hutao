// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class CookBonus
{
    public MaterialId OriginItemId { get; set; } = default!;

    public MaterialId ItemId { get; set; } = default!;

    public ImmutableArray<MaterialId> InputList { get; set; } = default!;
}