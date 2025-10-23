// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Wiki;

internal sealed class WikiAvatarMetadataContext : IMetadataContext,
    IMetadataDictionaryLevelAvatarGrowCurveSource,
    IMetadataDictionaryIdDictionaryLevelAvatarPromoteSource,
    IMetadataDictionaryIdMaterialSource,
    IMetadataDictionaryIdHyperLinkNameSource,
    IMetadataArrayAvatarSource
{
    public ImmutableDictionary<Level, TypeValueCollection<GrowCurveType, float>> LevelDictionaryAvatarGrowCurveMap { get; set; } = default!;

    public ImmutableDictionary<PromoteId, ImmutableDictionary<PromoteLevel, Promote>> IdDictionaryAvatarLevelPromoteMap { get; set; } = default!;

    public ImmutableDictionary<MaterialId, Material> IdMaterialMap { get; set; } = default!;

    public ImmutableDictionary<HyperLinkNameId, HyperLinkName> IdHyperLinkNameMap { get; set; } = default!;

    public ImmutableArray<Avatar> Avatars { get; set; }
}