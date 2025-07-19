// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Wiki;

internal sealed class WikiMonsterMetadataContext : IMetadataContext,
    IMetadataDictionaryLevelMonsterGrowCurveSource,
    IMetadataArrayMonsterSource,
    IMetadataDictionaryIdDisplayItemAndMaterialSource
{
    public ImmutableDictionary<Level, TypeValueCollection<GrowCurveType, float>> LevelDictionaryMonsterGrowCurveMap { get; set; } = default!;

    public ImmutableArray<Monster> Monsters { get; set; } = default!;

    public ImmutableDictionary<MaterialId, DisplayItem> IdDisplayItemAndMaterialMap { get; set; } = default!;
}