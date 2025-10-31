// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using WinRT;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<string>("Text", PropertyChangedCallbackName = nameof(OnTextChanged))]
internal sealed partial class ReconfirmDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial ReconfirmDialog(IServiceProvider serviceProvider);

    public string ConfirmText { get; private set; } = default!;

    public async ValueTask<bool> ConfirmAsync(string confirmText)
    {
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        ConfirmText = confirmText;
        return await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary;
    }

    private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ReconfirmDialog dialog = sender.As<ReconfirmDialog>();
        dialog.IsPrimaryButtonEnabled = string.Equals(dialog.Text, dialog.ConfirmText, StringComparison.Ordinal);
    }
}