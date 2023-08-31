// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃 API 服务
/// </summary>
[HighQuality]
internal interface IHutaoSpiralAbyssService
{
    /// <summary>
    /// 异步获取角色上场率
    /// </summary>
    /// <returns>角色上场率</returns>
    ValueTask<List<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync();

    /// <summary>
    /// 异步获取角色搭配
    /// </summary>
    /// <returns>角色搭配</returns>
    ValueTask<List<AvatarCollocation>> GetAvatarCollocationsAsync();

    /// <summary>
    /// 异步获取角色持有率信息
    /// </summary>
    /// <returns>角色持有率信息</returns>
    ValueTask<List<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync();

    /// <summary>
    /// 异步获取角色使用率
    /// </summary>
    /// <returns>角色使用率</returns>
    ValueTask<List<AvatarUsageRank>> GetAvatarUsageRanksAsync();

    /// <summary>
    /// 异步获取统计数据
    /// </summary>
    /// <returns>统计数据</returns>
    ValueTask<Overview> GetOverviewAsync();

    /// <summary>
    /// 异步获取队伍上场
    /// </summary>
    /// <returns>队伍上场</returns>
    ValueTask<List<TeamAppearance>> GetTeamAppearancesAsync();

    /// <summary>
    /// 异步获取武器搭配
    /// </summary>
    /// <returns>武器搭配</returns>
    ValueTask<List<WeaponCollocation>> GetWeaponCollocationsAsync();
}