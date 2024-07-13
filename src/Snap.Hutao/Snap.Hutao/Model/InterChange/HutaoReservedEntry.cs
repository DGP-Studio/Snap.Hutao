// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange;

internal sealed class HutaoReservedEntry<TItem>
{
    public required string Identity { get; set; }

    public required List<TItem> List { get; set; }
}