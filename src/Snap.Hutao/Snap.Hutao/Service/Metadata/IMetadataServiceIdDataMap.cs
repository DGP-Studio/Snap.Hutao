// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata;

internal interface IMetadataServiceIdDataMap
{
    /// <summary>
    /// 异步获取装备被动Id到圣遗物套装的映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>装备被动Id到圣遗物套装的映射</returns>
    ValueTask<Dictionary<EquipAffixId, ReliquarySet>> GetEquipAffixIdToReliquarySetMapAsync(CancellationToken token = default);

    ValueTask<Dictionary<ExtendedEquipAffixId, ReliquarySet>> GetExtendedEquipAffixIdToReliquarySetMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取成就映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>成就映射</returns>
    ValueTask<Dictionary<AchievementId, Model.Metadata.Achievement.Achievement>> GetIdToAchievementMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取Id到角色的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>Id到角色的字典</returns>
    ValueTask<Dictionary<AvatarId, Avatar>> GetIdToAvatarMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取显示与材料映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>显示与材料映射</returns>
    ValueTask<Dictionary<MaterialId, DisplayItem>> GetIdToDisplayItemAndMaterialMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取Id到材料的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>Id到材料的字典</returns>
    ValueTask<Dictionary<MaterialId, Material>> GetIdToMaterialMapAsync(CancellationToken token = default(CancellationToken));

    /// <summary>
    /// 异步获取Id到圣遗物权重的映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>Id到圣遗物权重的字典</returns>
    ValueTask<Dictionary<AvatarId, ReliquaryAffixWeight>> GetIdToReliquaryAffixWeightMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取ID到圣遗物副词条的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>字典</returns>
    ValueTask<Dictionary<ReliquarySubAffixId, ReliquarySubAffix>> GetIdToReliquarySubAffixMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物主词条Id与属性的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>字典</returns>
    ValueTask<Dictionary<ReliquaryMainAffixId, FightProperty>> GetIdToReliquaryMainPropertyMapAsync(CancellationToken token = default);

    ValueTask<Dictionary<TowerScheduleId, TowerSchedule>> GetIdToTowerScheduleMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取ID到武器的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>Id到武器的字典</returns>
    ValueTask<Dictionary<WeaponId, Weapon>> GetIdToWeaponMapAsync(CancellationToken token = default);

    ValueTask<Dictionary<TowerLevelGroupId, List<TowerLevel>>> GetGroupIdToTowerLevelGroupMapAsync(CancellationToken token = default);

    ValueTask<Dictionary<TowerFloorId, TowerFloor>> GetIdToTowerFloorMapAsync(CancellationToken token = default);

    ValueTask<Dictionary<MonsterRelationshipId, Monster>> GetRelationshipIdToMonsterMapAsync(CancellationToken token = default);
}
