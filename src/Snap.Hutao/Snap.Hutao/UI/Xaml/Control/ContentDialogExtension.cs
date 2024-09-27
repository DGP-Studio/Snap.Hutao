// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;

namespace Snap.Hutao.UI.Xaml.Control;

internal static class ContentDialogExtension
{
    public static async ValueTask<ContentDialogScope> BlockAsync(this ContentDialog contentDialog, IContentDialogFactory contentDialogFactory)
    {
        TaskCompletionSource dialogShowSource = new();
        _ = contentDialogFactory.EnqueueAndShowAsync(contentDialog, dialogShowSource);
        contentDialog.DispatcherQueue.TryEnqueue(() => contentDialog.Focus(FocusState.Programmatic));
        await dialogShowSource.Task.ConfigureAwait(false);
        return new ContentDialogScope(contentDialog, contentDialogFactory.TaskContext);
    }
}