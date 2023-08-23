// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃 API 缓存
/// </summary>
[HighQuality]
internal interface IHutaoCache
{
    /// <summary>
    /// 角色使用率
    /// </summary>
    List<AvatarRankView>? AvatarUsageRanks { get; set; }

    /// <summary>
    /// 角色上场率
    /// </summary>
    List<AvatarRankView>? AvatarAppearanceRanks { get; set; }

    /// <summary>
    /// 角色命座信息
    /// </summary>
    List<AvatarConstellationInfoView>? AvatarConstellationInfos { get; set; }

    /// <summary>
    /// 队伍出场
    /// </summary>
    List<TeamAppearanceView>? TeamAppearances { get; set; }

    /// <summary>
    /// 总览数据
    /// </summary>
    Overview? Overview { get; set; }

    /// <summary>
    /// 角色搭配
    /// </summary>
    Dictionary<AvatarId, AvatarCollocationView>? AvatarCollocations { get; set; }

    /// <summary>
    /// 武器搭配
    /// </summary>
    Dictionary<WeaponId, WeaponCollocationView>? WeaponCollocations { get; set; }

    /// <summary>
    /// 为数据库视图模型初始化
    /// </summary>
    /// <returns>是否初始化完成</returns>
    ValueTask<bool> InitializeForDatabaseViewModelAsync();

    /// <summary>
    /// 为Wiki角色视图模型初始化
    /// </summary>
    /// <returns>是否初始化完成</returns>
    ValueTask<bool> InitializeForWikiAvatarViewModelAsync();

    /// <summary>
    /// 为Wiki武器视图模型初始化
    /// </summary>
    /// <returns>是否初始化完成</returns>
    ValueTask<bool> InitializeForWikiWeaponViewModelAsync();
}