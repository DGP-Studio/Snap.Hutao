// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Reliquary;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataListReliquaryMainAffixLevelSource
{
    List<ReliquaryMainAffixLevel> ReliquaryMainAffixLevels { get; set; }
}