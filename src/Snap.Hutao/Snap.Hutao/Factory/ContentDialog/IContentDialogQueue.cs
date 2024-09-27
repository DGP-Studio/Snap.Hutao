// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Factory.ContentDialog;

[SuppressMessage("", "SH003")]
internal interface IContentDialogQueue
{
    bool IsDialogShowing { get; }

    Task<ContentDialogResult> EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog, TaskCompletionSource? dialogShowSource = null);
}