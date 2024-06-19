// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataListMaterialSource
{
    List<Material> Materials { get; set; }
}