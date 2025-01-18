// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal sealed class UIIFReliquary
{
    [JsonPropertyName("level")]
    public ReliquaryLevel Level { get; set; }

    [JsonPropertyName("mainPropId")]
    public ReliquaryMainAffixId MainPropId { get; set; }

    [JsonPropertyName("appendPropIdList")]
    public ImmutableArray<ReliquarySubAffixId> AppendPropIdList { get; set; } = default!;
}