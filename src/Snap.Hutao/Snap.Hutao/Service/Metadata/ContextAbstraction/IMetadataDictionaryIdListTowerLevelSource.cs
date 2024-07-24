// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryIdListTowerLevelSource
{
    Dictionary<TowerLevelGroupId, List<TowerLevel>> IdListTowerLevelMap { get; set; }
}