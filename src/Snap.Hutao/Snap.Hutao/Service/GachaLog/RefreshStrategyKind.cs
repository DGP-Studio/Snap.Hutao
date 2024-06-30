// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 刷新策略
/// </summary>
[HighQuality]
internal enum RefreshStrategyKind
{
    /// <summary>
    /// 无策略 用于切换存档时使用
    /// </summary>
    None = 0,

    /// <summary>
    /// 贪婪合并
    /// </summary>
    AggressiveMerge = 1,

    /// <summary>
    /// 懒惰合并
    /// </summary>
    LazyMerge = 2,
}