// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.Inventory;

// ReSharper disable once InconsistentNaming
internal sealed class UIIFCountInfo
{
    [JsonPropertyName("count")]
    public uint Count { get; set; }
}