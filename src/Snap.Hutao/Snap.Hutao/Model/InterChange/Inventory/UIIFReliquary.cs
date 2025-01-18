// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;
using PlayerStoreReliquary = Snap.Hutao.Service.Yae.PlayerStore.Reliquary;

namespace Snap.Hutao.Model.InterChange.Inventory;

// ReSharper disable once InconsistentNaming
internal sealed class UIIFReliquary
{
    [JsonPropertyName("level")]
    public ReliquaryLevel Level { get; set; }

    [JsonPropertyName("mainPropId")]
    public ReliquaryMainAffixId MainPropId { get; set; }

    [JsonPropertyName("appendPropIdList")]
    public ImmutableArray<ReliquarySubAffixId> AppendPropIdList { get; set; }

    public static UIIFReliquary FromPlayerStoreReliquary(PlayerStoreReliquary reliquary)
    {
        return new()
        {
            Level = reliquary.Level,
            MainPropId = reliquary.MainPropId,
            AppendPropIdList = reliquary.AppendPropIdList.Select(id => (ReliquarySubAffixId)id).ToImmutableArray(),
        };
    }
}