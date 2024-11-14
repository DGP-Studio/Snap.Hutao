// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class CookBonus
{
    public MaterialId OriginItemId { get; set; } = default!;

    public MaterialId ItemId { get; set; } = default!;

    public List<MaterialId> InputList { get; set; } = default!;
}