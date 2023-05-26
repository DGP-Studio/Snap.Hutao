// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 解锁时机选项
/// </summary>
internal readonly struct UnlockTimingOptions
{
    /// <summary>
    /// 每次查找 Module 的延时
    /// </summary>
    public readonly TimeSpan FindModuleDelay;

    /// <summary>
    /// 查找 Module 的最大时间阈值
    /// </summary>
    public readonly TimeSpan FindModuleLimit;

    /// <summary>
    /// 每次循环调整的间隔时间
    /// </summary>
    public readonly TimeSpan AdjustFpsDelay;

    /// <summary>
    /// 构造一个新的解锁器选项
    /// </summary>
    /// <param name="findModuleDelayMilliseconds">每次查找UnityPlayer的延时,推荐100毫秒</param>
    /// <param name="findModuleLimitMilliseconds">查找UnityPlayer的最大阈值,推荐10000毫秒</param>
    /// <param name="adjustFpsDelayMilliseconds">每次循环调整的间隔时间，推荐2000毫秒</param>
    public UnlockTimingOptions(int findModuleDelayMilliseconds, int findModuleLimitMilliseconds, int adjustFpsDelayMilliseconds)
    {
        FindModuleDelay = TimeSpan.FromMilliseconds(findModuleDelayMilliseconds);
        FindModuleLimit = TimeSpan.FromMilliseconds(findModuleLimitMilliseconds);
        AdjustFpsDelay = TimeSpan.FromMilliseconds(adjustFpsDelayMilliseconds);
    }
}