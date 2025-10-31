// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<string>("RedeemCode")]
internal sealed partial class HutaoPassportUseRedeemCodeDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial HutaoPassportUseRedeemCodeDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, string?>> GetInputAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, RedeemCode);
    }
}