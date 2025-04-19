// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Achievement;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

internal sealed partial class CloseButtonBehaviorSetDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public CloseButtonBehaviorSetDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();
    }

    public async ValueTask<ValueResult<bool, CloseButtonBehavior>> GetCloseButtonBehaviorAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, (CloseButtonBehavior)CloseButtonBehaviorSelector.SelectedIndex);
    }
}