// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Weapon;

internal sealed class NameDescriptions
{
    public required string Name { get; init; }

    public required ImmutableArray<LevelDescription> Descriptions { get; init; }
}