// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model;

internal class Item : INameIcon
{
    public string Name { get; set; } = default!;

    public Uri Icon { get; set; } = default!;

    public Uri Badge { get; set; } = default!;

    public QualityType Quality { get; set; }

    internal uint Id { get; set; }
}