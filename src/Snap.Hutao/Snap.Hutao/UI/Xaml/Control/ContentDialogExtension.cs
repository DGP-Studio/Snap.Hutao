// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;

namespace Snap.Hutao.UI.Xaml.Control;

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
    [Obsolete("Use another overload")]
    [SuppressMessage("", "SH100")]
    public static async ValueTask<ContentDialogScope> BlockAsync(this ContentDialog contentDialog, ITaskContext taskContext)
    {
        await taskContext.SwitchToMainThreadAsync();

        // E_ASYNC_OPERATION_NOT_STARTED 0x80000019
        // Only a single ContentDialog can be open at any time.
        _ = contentDialog.ShowAsync();
        return new ContentDialogScope(contentDialog, taskContext);
    }

    public static async ValueTask<ContentDialogScope> BlockAsync(this ContentDialog contentDialog, IContentDialogFactory contentDialogFactory)
    {
        TaskCompletionSource dialogShowSource = new();
        _ = contentDialogFactory.EnqueueAndShowAsync(contentDialog, dialogShowSource);
        await dialogShowSource.Task.ConfigureAwait(false);
        return new ContentDialogScope(contentDialog, contentDialogFactory.TaskContext);
    }
}