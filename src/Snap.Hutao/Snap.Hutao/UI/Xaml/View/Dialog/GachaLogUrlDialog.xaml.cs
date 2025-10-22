// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.UI.Xaml.View.Window;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty<string>("Text")]
internal sealed partial class GachaLogUrlDialog : ContentDialog
{
    private readonly IContentDialogFactory<MainWindow> contentDialogFactory;

    public async ValueTask<ValueResult<bool, string?>> GetInputUrlAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        string? url = Text?.TrimEnd("#/log");
        return new(result is ContentDialogResult.Primary, url);
    }
}