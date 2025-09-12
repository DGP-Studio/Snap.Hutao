// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Factory.ContentDialog;

internal static class ContentDialogFactoryExtension
{
    public static async ValueTask<BlockDeferral> BlockAsync(this IContentDialogFactory contentDialogFactory, Microsoft.UI.Xaml.Controls.ContentDialog contentDialog)
    {
        ValueContentDialogTask dialogTask = contentDialogFactory.EnqueueAndShowAsync(contentDialog);
        await dialogTask.QueueTask.ConfigureAwait(false);
        contentDialog.DispatcherQueue.Invoke(() => contentDialog.Focus(FocusState.Programmatic));
        return new(contentDialog);
    }
}