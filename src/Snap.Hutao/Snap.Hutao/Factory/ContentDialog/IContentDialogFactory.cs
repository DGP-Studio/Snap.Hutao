// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Factory.ContentDialog;

internal interface IContentDialogFactory
{
    bool IsDialogShowing { get; }

    ITaskContext TaskContext { get; }

    ValueTask<ContentDialogResult> CreateForConfirmAsync([LocalizationRequired] string title, [LocalizationRequired] string content);

    ValueTask<ContentDialogResult> CreateForConfirmCancelAsync([LocalizationRequired] string title, [LocalizationRequired] string content, ContentDialogButton defaultButton = ContentDialogButton.Close, bool isPrimaryButtonEnabled = true);

    ValueTask<Microsoft.UI.Xaml.Controls.ContentDialog> CreateForIndeterminateProgressAsync([LocalizationRequired] string title);

    ValueTask<TContentDialog> CreateInstanceAsync<TContentDialog>(IServiceProvider serviceProvider, params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog;

    ValueContentDialogTask EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog);
}