// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.GachaLog;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<GachaLogFetchStatus>("Status")]
internal sealed partial class GachaLogRefreshProgressDialog : ContentDialog
{
    public GachaLogRefreshProgressDialog()
    {
        InitializeComponent();
    }

    public void OnReport(GachaLogFetchStatus status)
    {
        Status = status;
    }
}