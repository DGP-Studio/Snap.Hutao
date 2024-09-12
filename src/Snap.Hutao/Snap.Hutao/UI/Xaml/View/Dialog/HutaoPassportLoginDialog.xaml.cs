// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("UserName", typeof(string))]
[DependencyProperty("Password", typeof(string))]
internal sealed partial class HutaoPassportLoginDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public HutaoPassportLoginDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    public async ValueTask<ValueResult<bool, (string UserName, string Passport)>> GetInputAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();

        return new(result is ContentDialogResult.Primary, (UserName, Password));
    }
}