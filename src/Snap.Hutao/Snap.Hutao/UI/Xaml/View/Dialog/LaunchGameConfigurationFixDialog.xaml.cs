// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("KnownSchemes", typeof(IEnumerable<LaunchScheme>))]
[DependencyProperty("SelectedScheme", typeof(LaunchScheme))]
internal sealed partial class LaunchGameConfigurationFixDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public LaunchGameConfigurationFixDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    public async ValueTask<ValueResult<bool, LaunchScheme>> GetLaunchSchemeAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        return new(result is ContentDialogResult.Primary, SelectedScheme);
    }
}
