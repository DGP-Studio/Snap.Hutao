// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.UI.Xaml.View.Window;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty<string>("UserName")]
[DependencyProperty<string>("Password")]
internal sealed partial class HutaoPassportLoginDialog : ContentDialog
{
    private readonly IContentDialogFactory<MainWindow> contentDialogFactory;

    public async ValueTask<ValueResult<bool, (string? UserName, string? Passport)>> GetInputAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, (UserName, Password));
    }
}