// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("Mirrors", typeof(List<HutaoPackageMirror>))]
[DependencyProperty("SelectedItem", typeof(HutaoPackageMirror))]
internal sealed partial class UpdatePackageDownloadConfirmDialog : ContentDialog
{
    public UpdatePackageDownloadConfirmDialog()
    {
        InitializeComponent();
    }

    public async ValueTask<ValueResult<bool, HutaoPackageMirror?>> GetSelectedMirrorAsync()
    {
        if (await ShowAsync() is ContentDialogResult.Primary)
        {
            return new(true, SelectedItem ?? Mirrors?.FirstOrDefault());
        }

        return new(false, default);
    }
}