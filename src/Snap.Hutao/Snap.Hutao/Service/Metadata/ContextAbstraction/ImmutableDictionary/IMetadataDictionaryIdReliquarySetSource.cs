// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;

internal interface IMetadataDictionaryIdReliquarySetSource
{
    ImmutableDictionary<ReliquarySetId, ReliquarySet> IdReliquarySetMap { get; set; }
}