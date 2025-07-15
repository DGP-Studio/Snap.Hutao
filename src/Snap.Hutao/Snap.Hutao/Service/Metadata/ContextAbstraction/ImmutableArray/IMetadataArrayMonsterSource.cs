// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Monster;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;

internal interface IMetadataArrayMonsterSource
{
    ImmutableArray<Monster> Monsters { get; set; }
}