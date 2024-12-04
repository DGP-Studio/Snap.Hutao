// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Factory.ContentDialog;

internal readonly struct ValueContentDialogTask
{
    /// <summary>
    /// This task will be completed when the associated dialog finishes queueing and starts to show.
    /// </summary>
    public readonly Task QueueTask;

    /// <summary>
    /// This task will be completed when the associated dialog closed in any reason.
    /// </summary>
    public readonly Task<ContentDialogResult> ShowTask;

    public ValueContentDialogTask(Task queueTask, Task<ContentDialogResult> showTask)
    {
        QueueTask = queueTask;
        ShowTask = showTask;
    }
}