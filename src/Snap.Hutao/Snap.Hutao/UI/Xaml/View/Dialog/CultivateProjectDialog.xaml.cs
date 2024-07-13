// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.User;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("Text", typeof(string))]
[DependencyProperty("IsUidAttached", typeof(bool))]
internal sealed partial class CultivateProjectDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public CultivateProjectDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    public async ValueTask<ValueResult<bool, CultivateProject>> CreateProjectAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        if (await ShowAsync() is ContentDialogResult.Primary)
        {
            return new(true, CultivateProject.From(Text));
        }

        return new(false, default!);
    }
}
