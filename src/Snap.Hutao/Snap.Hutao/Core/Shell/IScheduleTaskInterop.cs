// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Shell;

/// <summary>
/// 任务计划互操作
/// </summary>
internal interface IScheduleTaskInterop
{
    bool IsDailyNoteRefreshEnabled();

    /// <summary>
    /// 注册实时便笺刷新任务
    /// </summary>
    /// <param name="interval">间隔（秒）</param>
    /// <returns>是否注册或修改成功</returns>
    bool RegisterForDailyNoteRefresh(int interval);

    /// <summary>
    /// 卸载全部注册的任务
    /// </summary>
    /// <returns>是否卸载成功</returns>
    bool UnregisterAllTasks();

    bool UnregisterForDailyNoteRefresh();
}