// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty("Mirrors", typeof(List<HutaoPackageMirror>))]
[DependencyProperty("SelectedItem", typeof(HutaoPackageMirror))]
internal sealed partial class UpdatePackageDownloadConfirmDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public async ValueTask<ValueResult<bool, HutaoPackageMirror?>> GetSelectedMirrorAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary)
        {
            await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
            return new(true, SelectedItem ?? Mirrors?.FirstOrDefault());
        }

        return new(false, default);
    }
}