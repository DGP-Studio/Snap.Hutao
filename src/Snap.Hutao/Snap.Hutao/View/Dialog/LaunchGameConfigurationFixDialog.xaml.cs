// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.View.Dialog;

[DependencyProperty("KnownSchemes", typeof(IEnumerable<LaunchScheme>))]
[DependencyProperty("SelectedScheme", typeof(LaunchScheme))]
internal sealed partial class LaunchGameConfigurationFixDialog : ContentDialog
{
    public LaunchGameConfigurationFixDialog()
    {
        InitializeComponent();
    }

    public async ValueTask<ValueResult<bool, LaunchScheme?>> GetLaunchSchemeAsync()
    {
        ContentDialogResult result = await ShowAsync();

        return new(result == ContentDialogResult.Primary, SelectedScheme);
    }
}
