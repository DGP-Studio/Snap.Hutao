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
}