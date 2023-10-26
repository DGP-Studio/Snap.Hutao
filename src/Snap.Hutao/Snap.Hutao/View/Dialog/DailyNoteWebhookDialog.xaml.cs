// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Dialog;

[DependencyProperty("Text", typeof(string))]
internal sealed partial class DailyNoteWebhookDialog : ContentDialog
{
    private readonly ITaskContext taskContext;

    public DailyNoteWebhookDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    /// <summary>
    /// 获取输入的Url
    /// </summary>
    /// <returns>输入的结果</returns>
    public async ValueTask<ValueResult<bool, string>> GetInputUrlAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();

        return new(result == ContentDialogResult.Primary, Text);
    }
}