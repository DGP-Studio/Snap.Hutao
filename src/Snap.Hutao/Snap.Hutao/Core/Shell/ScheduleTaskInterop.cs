// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.TaskScheduler;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.Shell;

/// <summary>
/// 任务计划互操作
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient, typeof(IScheduleTaskInterop))]
internal sealed class ScheduleTaskInterop : IScheduleTaskInterop
{
    private const string DailyNoteRefreshTaskName = "SnapHutaoDailyNoteRefreshTask";

    /// <summary>
    /// 卸载全部注册的任务
    /// </summary>
    /// <returns>是否卸载成功</returns>
    public bool UnregisterAllTasks()
    {
        try
        {
            TaskService.Instance.RootFolder.DeleteTask(DailyNoteRefreshTaskName, false);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (COMException)
        {
            return false;
        }
    }
}