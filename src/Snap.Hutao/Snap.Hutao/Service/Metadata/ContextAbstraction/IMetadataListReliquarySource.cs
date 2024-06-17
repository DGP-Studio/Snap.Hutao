// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Reliquary;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataListReliquarySource
{
    List<Reliquary> Reliquaries { get; set; }
}