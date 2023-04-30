// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// 游戏位置定位器
/// </summary>
[HighQuality]
internal interface IGameLocator : INamedService
{
    /// <summary>
    /// 异步获取游戏位置
    /// 路径应当包含游戏文件名称
    /// </summary>
    /// <returns>游戏位置</returns>
    Task<ValueResult<bool, string>> LocateGamePathAsync();
}