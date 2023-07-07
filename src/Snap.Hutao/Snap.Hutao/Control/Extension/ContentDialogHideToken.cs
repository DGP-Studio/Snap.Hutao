// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Extension;

internal readonly struct ContentDialogHideToken : IDisposable
{
    private readonly ContentDialog contentDialog;
    private readonly ITaskContext taskContext;

    public ContentDialogHideToken(ContentDialog contentDialog, ITaskContext taskContext)
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