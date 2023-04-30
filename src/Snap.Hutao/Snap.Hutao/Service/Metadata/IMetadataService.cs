// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据服务
/// </summary>
[HighQuality]
internal interface IMetadataService : ICastableService
{
    /// <summary>
    /// 异步初始化服务，尝试更新元数据
    /// </summary>
    /// <returns>初始化是否成功</returns>
    ValueTask<bool> InitializeAsync();

    /// <summary>
    /// 异步获取成就列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>成就列表</returns>
    ValueTask<List<Model.Metadata.Achievement.Achievement>> GetAchievementsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取成就分类列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>成就分类列表</returns>
    ValueTask<List<AchievementGoal>> GetAchievementGoalsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取角色列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    ValueTask<List<Avatar>> GetAvatarsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取装备被动Id到圣遗物套装的映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>装备被动Id到圣遗物套装的映射</returns>
    ValueTask<Dictionary<EquipAffixId, ReliquarySet>> GetEquipAffixIdToReliquarySetMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取卡池配置列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>卡池配置列表</returns>
    ValueTask<List<GachaEvent>> GetGachaEventsAsync(CancellationToken token = default);

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
    ValueTask<Dictionary<MaterialId, Display>> GetIdToDisplayAndMaterialMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取Id到材料的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>Id到材料的字典</returns>
    ValueTask<Dictionary<MaterialId, Material>> GetIdToMaterialMapAsync(CancellationToken token = default(CancellationToken));

    /// <summary>
    /// 异步获取ID到圣遗物副词条的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>字典</returns>
    ValueTask<Dictionary<ReliquaryAffixId, ReliquaryAffix>> GetIdToReliquaryAffixMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物主词条Id与属性的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>字典</returns>
    ValueTask<Dictionary<ReliquaryMainAffixId, FightProperty>> GetIdToReliquaryMainPropertyMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取ID到武器的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>Id到武器的字典</returns>
    ValueTask<Dictionary<WeaponId, Weapon>> GetIdToWeaponMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取名称到角色的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>名称到角色的字典</returns>
    ValueTask<Dictionary<string, Avatar>> GetNameToAvatarMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取名称到武器的字典
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>名称到武器的字典</returns>
    ValueTask<Dictionary<string, Weapon>> GetNameToWeaponMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物列表</returns>
    ValueTask<List<Reliquary>> GetReliquariesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物强化属性列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物强化属性列表</returns>
    ValueTask<List<ReliquaryAffix>> GetReliquaryAffixesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物等级数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物等级数据</returns>
    ValueTask<List<ReliquaryLevel>> GetReliquaryLevelsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物主属性强化属性列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物强化属性列表</returns>
    ValueTask<List<ReliquaryMainAffix>> GetReliquaryMainAffixesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取武器列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>武器列表</returns>
    ValueTask<List<Weapon>> GetWeaponsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物套装
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物套装列表</returns>
    ValueTask<List<ReliquarySet>> GetReliquarySetsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取材料列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>材料列表</returns>
    ValueTask<List<Material>> GetMaterialsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取怪物列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>怪物列表</returns>
    ValueTask<List<Monster>> GetMonstersAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取等级角色曲线映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>等级角色曲线映射</returns>
    ValueTask<Dictionary<int, Dictionary<GrowCurveType, float>>> GetLevelToAvatarCurveMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取等级怪物曲线映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>等级怪物曲线映射</returns>
    ValueTask<Dictionary<int, Dictionary<GrowCurveType, float>>> GetLevelToMonsterCurveMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取等级武器曲线映射
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>等级武器曲线映射</returns>
    ValueTask<Dictionary<int, Dictionary<GrowCurveType, float>>> GetLevelToWeaponCurveMapAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取角色突破列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色突破列表</returns>
    ValueTask<List<Promote>> GetAvatarPromotesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取武器突破列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>武器突破列表</returns>
    ValueTask<List<Promote>> GetWeaponPromotesAsync(CancellationToken token = default);
}