// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Factory.ContentDialog;

internal struct ContentDialogScope : IDisposable
{
    private readonly Microsoft.UI.Xaml.Controls.ContentDialog contentDialog;

    private bool disposing = false;
    private bool disposed = false;

    public ContentDialogScope(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog)
    {
        this.contentDialog = contentDialog;
    }

    public void Dispose()
    {
        if (!disposed && !disposing)
        {
            disposing = true;
            contentDialog.DispatcherQueue.Invoke(contentDialog.Hide);
            disposing = false;
            disposed = true;
        }
    }
}