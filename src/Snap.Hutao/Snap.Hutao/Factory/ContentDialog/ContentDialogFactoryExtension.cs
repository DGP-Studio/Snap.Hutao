// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Factory.ContentDialog;

internal static class ContentDialogFactoryExtension
{
    public static async ValueTask<ContentDialogScope> BlockAsync(this IContentDialogFactory contentDialogFactory, Microsoft.UI.Xaml.Controls.ContentDialog contentDialog)
    {
        TaskCompletionSource dialogShowSource = new();
        ValueContentDialogTask dialogTask = contentDialogFactory.EnqueueAndShowAsync(contentDialog);
        await dialogTask.QueueTask.ConfigureAwait(false);
        contentDialog.DispatcherQueue.TryEnqueue(() => contentDialog.Focus(FocusState.Programmatic));
        return new(contentDialog);
    }
}