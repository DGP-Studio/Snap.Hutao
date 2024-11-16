// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.GachaLog;

internal sealed class UIGF
{
    [JsonPropertyName("info")]
    [JsonRequired]
    public UIGFInfo Info { get; set; } = default!;

    [JsonPropertyName("hk4e")]
    public ImmutableArray<UIGFEntry<Hk4eItem>> Hk4e { get; set; }
}