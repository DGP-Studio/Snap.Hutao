// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty("Text", typeof(string))]
[DependencyProperty("IsUidAttached", typeof(bool))]
internal sealed partial class CultivateProjectDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public async ValueTask<ValueResult<bool, CultivateProject>> CreateProjectAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary)
        {
            await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
            return new(true, CultivateProject.From(Text));
        }

        return new(false, default!);
    }
}
