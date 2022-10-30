// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
internal interface IGameService
{
    /// <summary>
    /// 异步获取游戏路径
    /// </summary>
    /// <returns>结果</returns>
    ValueTask<ValueResult<bool, string>> GetGamePathAsync();

    /// <summary>
    /// 获取游戏路径，跳过异步定位器
    /// </summary>
    /// <returns>游戏路径，当路径无效时会设置并返回 <see cref="string.Empty"/></returns>
    string GetGamePathSkipLocator();

    /// <summary>
    /// 获取多通道值
    /// </summary>
    /// <returns>多通道值</returns>
    MultiChannel GetMultiChannel();

    /// <summary>
    /// 异步启动
    /// </summary>
    /// <param name="configuration">启动配置</param>
    /// <returns>任务</returns>
    ValueTask LaunchAsync(LaunchConfiguration configuration);

    /// <summary>
    /// 重写游戏路径
    /// </summary>
    /// <param name="path">路径</param>
    void OverwriteGamePath(string path);
}