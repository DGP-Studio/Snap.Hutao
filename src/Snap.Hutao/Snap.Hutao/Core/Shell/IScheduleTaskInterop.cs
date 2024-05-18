// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Shell;

/// <summary>
/// 任务计划互操作
/// </summary>
internal interface IScheduleTaskInterop
{
    /// <summary>
    /// 卸载全部注册的任务
    /// </summary>
    /// <returns>是否卸载成功</returns>
    bool UnregisterAllTasks();
}