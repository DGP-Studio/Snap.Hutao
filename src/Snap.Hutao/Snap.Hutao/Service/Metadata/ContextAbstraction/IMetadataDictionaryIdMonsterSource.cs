// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryIdMonsterSource
{
    Dictionary<MonsterRelationshipId, Monster> IdMonsterMap { get; set; }
}