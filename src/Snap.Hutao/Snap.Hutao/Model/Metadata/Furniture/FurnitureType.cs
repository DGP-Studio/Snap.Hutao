// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Furniture;

internal sealed class FurnitureType
{
    public FurnitureTypeId Id { get; set; }

    public uint Category { get; set; }

    public string Name { get; set; } = default!;

    public string Name2 { get; set; } = default!;

    public string TabIcon { get; set; } = default!;

    public FurnitureDeployType SceneType { get; set; }

    public bool BagPageOnly { get; set; }

    public bool IsShowInBag { get; set; }

    public uint Sort { get; set; }
}