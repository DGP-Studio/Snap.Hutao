// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model;

internal class Item : INameIcon<Uri>
{
    public string Name { get; init; } = default!;

    public Uri Icon { get; init; } = default!;

    public Uri Badge { get; init; } = default!;

    public QualityType Quality { get; init; }

    internal uint Id { get; init; }
}