// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

internal sealed partial class SpiralAbyssUploadRecordHomaNotLoginDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public SpiralAbyssUploadRecordHomaNotLoginDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    public async ValueTask<bool> ConfirmAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        return await ShowAsync() is ContentDialogResult.Primary;
    }
}
