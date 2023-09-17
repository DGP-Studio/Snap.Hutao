// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed class SpiralAbyssMetadataContext
{
    public Dictionary<TowerScheduleId, TowerSchedule> IdScheduleMap { get; set; } = default!;

    public Dictionary<TowerFloorId, TowerFloor> IdFloorMap { get; set; } = default!;

    public Dictionary<TowerLevelGroupId, List<TowerLevel>> IdLevelGroupMap { get; set; } = default!;

    public Dictionary<MonsterRelationshipId, Monster> IdMonsterMap { get; set; } = default!;

    public Dictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;
}