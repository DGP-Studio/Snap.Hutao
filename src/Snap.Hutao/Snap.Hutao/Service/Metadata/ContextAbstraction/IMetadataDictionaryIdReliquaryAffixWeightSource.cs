// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryIdReliquaryAffixWeightSource
{
    public Dictionary<AvatarId, ReliquaryAffixWeight> IdReliquaryAffixWeightMap { get; set; }
}