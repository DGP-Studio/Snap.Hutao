// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.GachaLog;

internal sealed class UIGF
{
    [JsonPropertyName("info")]
    [JsonRequired]
    public UIGFInfo Info { get; set; } = default!;

    [JsonPropertyName("hk4e")]
    public List<UIGFEntry<Hk4eItem>>? Hk4e { get; set; }
}