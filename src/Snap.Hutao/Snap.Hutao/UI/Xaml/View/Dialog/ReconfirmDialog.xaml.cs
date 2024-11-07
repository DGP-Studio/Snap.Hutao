// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty("Text", typeof(string), default(string), nameof(OnTextChanged))]
internal sealed partial class ReconfirmDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public string ConfirmText { get; private set; } = default!;

    public async ValueTask<bool> ConfirmAsync(string confirmText)
    {
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        ConfirmText = confirmText;
        return await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary;
    }

    private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ReconfirmDialog dialog = (ReconfirmDialog)sender;
        dialog.IsPrimaryButtonEnabled = string.Equals(dialog.Text, dialog.ConfirmText, StringComparison.Ordinal);
    }
}
