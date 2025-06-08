// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

internal sealed class HyperLinkName
{
    public required HyperLinkNameId Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }
}