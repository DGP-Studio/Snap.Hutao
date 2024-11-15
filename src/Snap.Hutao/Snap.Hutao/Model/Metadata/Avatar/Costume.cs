﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class Costume
{
    public required CostumeId Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required bool IsDefault { get; init; }

    public string? FrontIcon { get; init; }

    public string? SideIcon { get; init; }
}