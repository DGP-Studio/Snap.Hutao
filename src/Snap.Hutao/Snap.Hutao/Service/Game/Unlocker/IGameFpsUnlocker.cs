// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 游戏帧率解锁器
/// </summary>
[HighQuality]
internal interface IGameFpsUnlocker
{
    /// <summary>
    /// 异步的解锁帧数限制
    /// </summary>
    /// <param name="findModuleDelay">每次查找UnityPlayer的延时,推荐100毫秒</param>
    /// <param name="findModuleLimit">查找UnityPlayer的最大阈值,推荐10000毫秒</param>
    /// <param name="adjustFpsDelay">每次循环调整的间隔时间，推荐2000毫秒</param>
    /// <returns>解锁的结果</returns>
    Task UnlockAsync(TimeSpan findModuleDelay, TimeSpan findModuleLimit, TimeSpan adjustFpsDelay);
}