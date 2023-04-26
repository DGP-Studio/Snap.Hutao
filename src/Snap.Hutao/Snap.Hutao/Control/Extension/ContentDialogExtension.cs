// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Extension;

/// <summary>
/// 对话框扩展
/// </summary>
[HighQuality]
internal static class ContentDialogExtension
{
    /// <summary>
    /// 阻止用户交互
    /// </summary>
    /// <param name="contentDialog">对话框</param>
    /// <param name="taskContext">任务上下文</param>
    /// <returns>用于恢复用户交互</returns>
    public static async ValueTask<IDisposable> BlockAsync(this ContentDialog contentDialog, ITaskContext taskContext)
    {
        await taskContext.SwitchToMainThreadAsync();
        contentDialog.ShowAsync().AsTask().SafeForget();

        // E_ASYNC_OPERATION_NOT_STARTED 0x80000019
        // Only a single ContentDialog can be open at any time.
        return new ContentDialogHider(contentDialog, taskContext);
    }
}

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1400")]
[SuppressMessage("", "SA1600")]
file readonly struct ContentDialogHider : IDisposable
{
    private readonly ContentDialog contentDialog;
    private readonly ITaskContext taskContext;

    public ContentDialogHider(ContentDialog contentDialog, ITaskContext taskContext)
    {
        this.contentDialog = contentDialog;
        this.taskContext = taskContext;
    }

    public void Dispose()
    {
        // Hide() must be called on main thread.
        taskContext.InvokeOnMainThread(contentDialog.Hide);
    }
}