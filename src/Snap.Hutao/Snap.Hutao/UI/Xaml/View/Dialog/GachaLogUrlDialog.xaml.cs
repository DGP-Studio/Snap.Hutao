// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<string>("Text")]
internal sealed partial class GachaLogUrlDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial GachaLogUrlDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, string?>> GetInputUrlAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        string? url = Text?.TrimEnd("#/log");
        return new(result is ContentDialogResult.Primary, url);
    }
}