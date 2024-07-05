// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("Text", typeof(string), default(string), nameof(OnTextChanged))]
internal sealed partial class ReconfirmDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public ReconfirmDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    public string ConfirmText { get; private set; } = default!;

    public async ValueTask<bool> ConfirmAsync(string confirmText)
    {
        await taskContext.SwitchToMainThreadAsync();
        ConfirmText = confirmText;
        return await ShowAsync() is ContentDialogResult.Primary;
    }

    private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ReconfirmDialog dialog = (ReconfirmDialog)sender;
        dialog.IsPrimaryButtonEnabled = string.Equals(dialog.Text, dialog.ConfirmText, StringComparison.Ordinal);
    }
}
