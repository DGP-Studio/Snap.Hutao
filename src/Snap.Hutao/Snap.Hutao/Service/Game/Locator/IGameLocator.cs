// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// 游戏位置定位器
/// </summary>
internal interface IGameLocator : INamed
{
    /// <summary>
    /// 异步获取游戏位置
    /// 路径应当包含游戏文件名称
    /// </summary>
    /// <returns>游戏位置</returns>
    Task<ValueResult<bool, string>> LocateGamePathAsync();

    /// <summary>
    /// 异步获取游戏启动器位置
    /// 路径应当包含启动器文件名称
    /// </summary>
    /// <returns>游戏启动器位置</returns>
    Task<ValueResult<bool, string>> LocateLauncherPathAsync();
}