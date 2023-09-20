// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;

namespace Snap.Hutao.Service.Metadata;

internal interface IMetadataServiceRawData
{
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
    /// 异步获取角色突破列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色突破列表</returns>
    ValueTask<List<Promote>> GetAvatarPromotesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取角色列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    ValueTask<List<Avatar>> GetAvatarsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取卡池配置列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>卡池配置列表</returns>
    ValueTask<List<GachaEvent>> GetGachaEventsAsync(CancellationToken token = default);

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
    ValueTask<List<ReliquarySubAffix>> GetReliquarySubAffixesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物等级数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物等级数据</returns>
    ValueTask<List<ReliquaryMainAffixLevel>> GetReliquaryLevelsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物主属性强化属性列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物强化属性列表</returns>
    ValueTask<List<ReliquaryMainAffix>> GetReliquaryMainAffixesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物套装
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物套装列表</returns>
    ValueTask<List<ReliquarySet>> GetReliquarySetsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取武器突破列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>武器突破列表</returns>
    ValueTask<List<Promote>> GetWeaponPromotesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取武器列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>武器列表</returns>
    ValueTask<List<Weapon>> GetWeaponsAsync(CancellationToken token = default);
}
