// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Furniture;

internal sealed class FurnitureSuite
{
    public FurnitureSuiteId Id { get; set; }

    public List<FurnitureTypeId> Types { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string ItemIcon { get; set; } = default!;

    public string? MapIcon { get; set; }

    public List<AvatarId>? FavoriteNpcs { get; set; }

    public List<FurnitureId> Units { get; set; } = default!;
}