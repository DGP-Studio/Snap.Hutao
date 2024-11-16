// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Game.Package;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("State", typeof(PackageConvertStatus))]
internal sealed partial class LaunchGamePackageConvertDialog : ContentDialog
{
    public LaunchGamePackageConvertDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        DataContext = this;
    }
}
