// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.TaskScheduler;
using System.IO;
using System.Runtime.InteropServices;
using SchedulerTask = Microsoft.Win32.TaskScheduler.Task;

namespace Snap.Hutao.Core;

/// <summary>
/// 任务计划器服务
/// </summary>
internal static class ScheduleTaskHelper
{
    private const string DailyNoteRefreshTaskName = "SnapHutaoDailyNoteRefreshTask";

    /// <summary>
    /// 注册实时便笺刷新任务
    /// </summary>
    /// <param name="interval">间隔（秒）</param>
    /// <returns>是否注册或修改成功</returns>
    public static bool RegisterForDailyNoteRefresh(int interval)
    {
        try
        {
            // TODO: 似乎可以不删除任务，直接注册已经包含了更新功能
            SchedulerTask? targetTask = TaskService.Instance.GetTask(DailyNoteRefreshTaskName);
            if (targetTask != null)
            {
                TaskService.Instance.RootFolder.DeleteTask(DailyNoteRefreshTaskName);
            }

            TaskDefinition task = TaskService.Instance.NewTask();
            task.RegistrationInfo.Description = "胡桃实时便笺刷新任务 | 请勿编辑或删除。";
            task.Triggers.Add(new TimeTrigger() { Repetition = new(TimeSpan.FromSeconds(interval), TimeSpan.Zero), });
            task.Actions.Add("explorer", "hutao://DailyNote/Refresh");
            TaskService.Instance.RootFolder.RegisterTaskDefinition(DailyNoteRefreshTaskName, task);
            return true;
        }
        catch (Exception ex)
        {
            _ = ex;
            return false;
        }
    }

    /// <summary>
    /// 卸载全部注册的任务
    /// </summary>
    /// <returns>是否卸载成功</returns>
    public static bool UnregisterAllTasks()
    {
        try
        {
            SchedulerTask? targetTask = TaskService.Instance.GetTask(DailyNoteRefreshTaskName);
            if (targetTask != null)
            {
                TaskService.Instance.RootFolder.DeleteTask(DailyNoteRefreshTaskName);
            }

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
