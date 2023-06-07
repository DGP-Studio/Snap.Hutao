// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 查找模块结果
/// </summary>
internal enum FindModuleResult
{
    /// <summary>
    /// 成功
    /// </summary>
    Ok,

    /// <summary>
    /// 超时
    /// </summary>
    TimeLimitExeeded,

    /// <summary>
    /// 模块尚未加载
    /// </summary>
    ModuleNotLoaded,

    /// <summary>
    /// 没有模块，保护驱动已加载，无法读取
    /// </summary>
    NoModuleFound,
}