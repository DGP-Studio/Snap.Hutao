// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Extension;

internal struct ContentDialogHideToken : IDisposable, IAsyncDisposable
{
    private readonly ContentDialog contentDialog;
    private readonly ITaskContext taskContext;

    private bool disposing = false;
    private bool disposed = false;

    public ContentDialogHideToken(ContentDialog contentDialog, ITaskContext taskContext)
    {
        this.contentDialog = contentDialog;
        this.taskContext = taskContext;
    }

    public void Dispose()
    {
        if (!disposed && !disposing)
        {
            disposing = true;
            taskContext.InvokeOnMainThread(contentDialog.Hide); // Hide() must be called on main thread.
            disposing = false;
            disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!disposed && !disposing)
        {
            disposing = true;
            await taskContext.SwitchToMainThreadAsync();
            contentDialog.Hide();
            disposing = false;
            disposed = true;
        }
    }
}