// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.GachaLog;

internal sealed class UIGF
{
    [JsonPropertyName("info")]
    public required UIGFInfo Info { get; set; }

    [JsonPropertyName("hk4e")]
    public List<UIGFEntry<Hk4eItem>>? Hk4e { get; set; }
}