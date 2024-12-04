// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Factory.ContentDialog;

[SuppressMessage("", "SH003")]
internal interface IContentDialogQueue
{
    bool IsDialogShowing { get; }

    ValueContentDialogTask EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog);
}