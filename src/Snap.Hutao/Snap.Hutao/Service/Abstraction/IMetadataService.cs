// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Metadata.Weapon;
using System.Collections.Generic;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据服务
/// </summary>
internal interface IMetadataService
{
    /// <summary>
    /// 异步初始化服务，尝试更新元数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>初始化是否成功</returns>
    ValueTask<bool> InitializeAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取成就列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>成就列表</returns>
    ValueTask<IEnumerable<Achievement>> GetAchievementsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取成就分类列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>成就分类列表</returns>
    ValueTask<IEnumerable<AchievementGoal>> GetAchievementGoalsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取角色列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    ValueTask<IEnumerable<Avatar>> GetAvatarsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物列表</returns>
    ValueTask<IEnumerable<Reliquary>> GetReliquariesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物强化属性列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物强化属性列表</returns>
    ValueTask<IEnumerable<ReliquaryAffix>> GetReliquaryAffixesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取圣遗物主属性强化属性列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物强化属性列表</returns>
    ValueTask<IEnumerable<ReliquaryAffixBase>> GetReliquaryMainAffixesAsync(CancellationToken token = default);

    /// <summary>
    /// 异步获取武器列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>武器列表</returns>
    ValueTask<IEnumerable<Weapon>> GetWeaponsAsync(CancellationToken token = default);

    /// <summary>
    /// 异步更新元数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>更新是否完成</returns>
    Task<bool> UpdateMetadataAsync(CancellationToken token = default);
}
