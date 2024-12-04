// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Furniture;

internal sealed class FurnitureMake
{
    public required FurnitureMakeId Id { get; init; }

    public required FurnitureId ItemId { get; init; }

    public required uint Experience { get; init; }

    public required ImmutableArray<IdCount> Materials { get; init; }
}