// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Furniture;

internal sealed class FurnitureMake
{
    public FurnitureMakeId Id { get; set; }

    public FurnitureId ItemId { get; set; }

    public uint Experience { get; set; }

    public List<IdCount> Materials { get; set; } = default!;
}