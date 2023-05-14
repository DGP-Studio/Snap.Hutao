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
    /// <param name="locateConfig"> 获取的配置 参数1：是否是选择 星穹铁道 的游戏路径 参数2：是否是选择 DLL 的路径</param>
    /// <returns>游戏位置</returns>
    Task<ValueResult<bool, string>> LocateGamePathAsync(ValueResult<bool, bool> locateConfig);
}