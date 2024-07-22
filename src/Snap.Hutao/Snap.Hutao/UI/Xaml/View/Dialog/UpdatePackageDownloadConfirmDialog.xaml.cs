// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("Mirrors", typeof(List<HutaoPackageMirror>))]
internal sealed partial class UpdatePackageDownloadConfirmDialog : ContentDialog
{
    public UpdatePackageDownloadConfirmDialog()
    {
        InitializeComponent();
    }
}