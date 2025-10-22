// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Factory.ContentDialog;

[SuppressMessage("", "SH003")]
internal interface IContentDialogQueue<TWindow>
    where TWindow : Microsoft.UI.Xaml.Window
{
    bool IsDialogShowing { get; }

    ValueContentDialogTask EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog);
}