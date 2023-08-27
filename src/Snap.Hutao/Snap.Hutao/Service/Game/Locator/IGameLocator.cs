// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// 游戏位置定位器
/// </summary>
[HighQuality]
internal interface IGameLocator
{
    /// <summary>
    /// 异步获取游戏位置
    /// 路径应当包含游戏文件名称
    /// </summary>
    /// <returns>游戏位置</returns>
    ValueTask<ValueResult<bool, string>> LocateGamePathAsync();
}