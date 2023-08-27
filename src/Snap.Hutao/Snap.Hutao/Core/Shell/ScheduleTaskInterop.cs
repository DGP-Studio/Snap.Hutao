// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.TaskScheduler;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Storage;

namespace Snap.Hutao.Core.Shell;

/// <summary>
/// 任务计划互操作
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient, typeof(IScheduleTaskInterop))]
internal sealed class ScheduleTaskInterop : IScheduleTaskInterop
{
    private const string DailyNoteRefreshTaskName = "SnapHutaoDailyNoteRefreshTask";
    private const string DailyNoteRefreshScriptName = "DailyNoteRefresh";

    /// <summary>
    /// 注册实时便笺刷新任务
    /// </summary>
    /// <param name="interval">间隔（秒）</param>
    /// <returns>是否注册或修改成功</returns>
    public bool RegisterForDailyNoteRefresh(int interval)
    {
        try
        {
            TaskDefinition task = TaskService.Instance.NewTask();
            task.RegistrationInfo.Description = SH.CoreScheduleTaskHelperDailyNoteRefreshTaskDescription;
            task.Triggers.Add(new TimeTrigger() { Repetition = new(TimeSpan.FromSeconds(interval), TimeSpan.Zero), });

            string scriptPath = EnsureWScriptCreated(DailyNoteRefreshScriptName, "hutao://DailyNote/Refresh");
            task.Actions.Add("wscript", $@"/b ""{scriptPath}""");

            TaskService.Instance.RootFolder.RegisterTaskDefinition(DailyNoteRefreshTaskName, task);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UnregisterForDailyNoteRefresh()
    {
        try
        {
            TaskService.Instance.RootFolder.DeleteTask(DailyNoteRefreshTaskName, false);
            if (WScriptExists(DailyNoteRefreshScriptName, out string fullPath))
            {
                File.Delete(fullPath);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool IsDailyNoteRefreshEnabled()
    {
        return WScriptExists(DailyNoteRefreshScriptName, out _);
    }

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

    private static string EnsureWScriptCreated(string name, string url, bool forceCreate = false)
    {
        if (WScriptExists(name, out string fullName) && !forceCreate)
        {
            return fullName;
        }

        string script = $"""CreateObject("WScript.Shell").Run "cmd /c start {url}", 0, False""";
        File.WriteAllText(fullName, script);

        return fullName;
    }

    private static bool WScriptExists(string name, out string fullName)
    {
        string tempFolder = ApplicationData.Current.TemporaryFolder.Path;
        fullName = Path.Combine(tempFolder, "Script", $"{name}.vbs");
        Directory.CreateDirectory(Path.Combine(tempFolder, "Script"));
        return File.Exists(fullName);
    }
}