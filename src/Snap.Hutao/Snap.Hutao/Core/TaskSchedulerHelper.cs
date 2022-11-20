// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.TaskScheduler;
using SchedulerTask = Microsoft.Win32.TaskScheduler.Task;

namespace Snap.Hutao.Core;

/// <summary>
/// 任务计划器服务
/// </summary>
internal static class TaskSchedulerHelper
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
            if (TaskService.Instance.GetTask(DailyNoteRefreshTaskName) is SchedulerTask targetTask)
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
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}
