// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[GeneratedConstructor(InitializeComponent = true)]
[DependencyProperty<string>("Text")]
internal sealed partial class LaunchGameAccountNameDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public LaunchGameAccountNameDialog(IServiceProvider serviceProvider, string originalName)
        : this(serviceProvider)
    {
        Text = originalName;
    }

    public async ValueTask<ValueResult<bool, string?>> GetInputNameAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary && !string.IsNullOrEmpty(Text), Text);
    }
}