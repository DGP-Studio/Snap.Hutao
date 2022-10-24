// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Web.Hutao.Model;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃 API 缓存
/// </summary>
internal interface IHtaoCache
{
    /// <summary>
    /// 角色使用率
    /// </summary>
    List<ComplexAvatarRank>? AvatarUsageRanks { get; set; }

    /// <summary>
    /// 角色上场率
    /// </summary>
    List<ComplexAvatarRank>? AvatarAppearanceRanks { get; set; }

    /// <summary>
    /// 角色命座信息
    /// </summary>
    List<ComplexAvatarConstellationInfo>? AvatarConstellationInfos { get; set; }

    /// <summary>
    /// 队伍出场
    /// </summary>
    List<ComplexTeamRank>? TeamAppearances { get; set; }

    /// <summary>
    /// 总览数据
    /// </summary>
    Overview? Overview { get; set; }

    /// <summary>
    /// 角色搭配
    /// </summary>
    List<ComplexAvatarCollocation>? AvatarCollocations { get; set; }

    /// <summary>
    /// 为数据库视图模型初始化
    /// </summary>
    /// <returns>任务</returns>
    ValueTask<bool> InitializeForDatabaseViewModelAsync();

    /// <summary>
    /// 为Wiki角色视图模型初始化
    /// </summary>
    /// <returns>任务</returns>
    ValueTask<bool> InitializeForWikiAvatarViewModelAsync();
}