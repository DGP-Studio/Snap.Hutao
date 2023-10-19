// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service;

namespace Snap.Hutao.View.Dialog;

[DependencyProperty("Text", typeof(string))]
internal sealed partial class GeetestCustomUrlDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public GeetestCustomUrlDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Text = serviceProvider.GetRequiredService<AppOptions>().GeetestCustomCompositeUrl;
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    public async ValueTask<ValueResult<bool, string>> GetUrlAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        return new(result == ContentDialogResult.Primary, Text ?? string.Empty);
    }
}
