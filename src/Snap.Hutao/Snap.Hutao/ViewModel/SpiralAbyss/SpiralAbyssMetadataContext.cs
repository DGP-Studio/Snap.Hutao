// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed class SpiralAbyssMetadataContext : IMetadataContext,
    IMetadataDictionaryIdTowerScheduleSource,
    IMetadataDictionaryIdTowerFloorSource,
    IMetadataDictionaryIdArrayTowerLevelSource,
    IMetadataDictionaryIdMonsterSource,
    IMetadataDictionaryIdAvatarWithPlayersSource
{
    public ImmutableDictionary<TowerScheduleId, TowerSchedule> IdTowerScheduleMap { get; set; } = default!;

    public ImmutableDictionary<TowerFloorId, TowerFloor> IdTowerFloorMap { get; set; } = default!;

    public ImmutableDictionary<TowerLevelGroupId, ImmutableArray<TowerLevel>> IdArrayTowerLevelMap { get; set; } = default!;

    public ImmutableDictionary<MonsterDescribeId, Monster> IdMonsterMap { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;
}