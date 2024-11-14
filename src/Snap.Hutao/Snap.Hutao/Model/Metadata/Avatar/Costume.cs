// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class Costume
{
    public CostumeId Id { get; set; }

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public bool IsDefault { get; set; }

    public string FrontIcon { get; set; } = default!;

    public string SideIcon { get; set; } = default!;
}