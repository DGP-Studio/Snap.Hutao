// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed class SpiralAbyssMetadataContext : IMetadataContext,
    IMetadataDictionaryIdTowerScheduleSource,
    IMetadataDictionaryIdTowerFloorSource,
    IMetadataDictionaryIdListTowerLevelSource,
    IMetadataDictionaryIdMonsterSource,
    IMetadataDictionaryIdAvatarWithPlayersSource
{
    public Dictionary<TowerScheduleId, TowerSchedule> IdTowerScheduleMap { get; set; } = default!;

    public Dictionary<TowerFloorId, TowerFloor> IdTowerFloorMap { get; set; } = default!;

    public Dictionary<TowerLevelGroupId, List<TowerLevel>> IdListTowerLevelMap { get; set; } = default!;

    public Dictionary<MonsterRelationshipId, Monster> IdMonsterMap { get; set; } = default!;

    public Dictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;
}