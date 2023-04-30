// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.Abstraction;

namespace Snap.Hutao.Factory;

/// <inheritdoc cref="IContentDialogFactory"/>
[HighQuality]
[Injection(InjectAs.Transient, typeof(IContentDialogFactory))]
internal sealed class ContentDialogFactory : IContentDialogFactory
{
    private readonly MainWindow mainWindow;
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的内容对话框工厂
    /// </summary>
    /// <param name="taskContext">任务上下文</param>
    /// <param name="mainWindow">主窗体</param>
    public ContentDialogFactory(ITaskContext taskContext, MainWindow mainWindow)
    {
        this.taskContext = taskContext;
        this.mainWindow = mainWindow;
    }

    /// <inheritdoc/>
    public async ValueTask<ContentDialogResult> ConfirmAsync(string title, string content)
    {
        ContentDialog dialog = await CreateForConfirmAsync(title, content).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        return await dialog.ShowAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<ContentDialogResult> ConfirmCancelAsync(string title, string content, ContentDialogButton defaultButton = ContentDialogButton.Close)
    {
        ContentDialog dialog = await CreateForConfirmCancelAsync(title, content, defaultButton).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        return await dialog.ShowAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<ContentDialog> CreateForIndeterminateProgressAsync(string title)
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialog dialog = new()
        {
            XamlRoot = mainWindow.Content.XamlRoot,
            Title = title,
            Content = new ProgressBar() { IsIndeterminate = true },
        };

        return dialog;
    }

    private async ValueTask<ContentDialog> CreateForConfirmAsync(string title, string content)
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialog dialog = new()
        {
            XamlRoot = mainWindow.Content.XamlRoot,
            Title = title,
            Content = content,
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText,
        };

        return dialog;
    }

    private async ValueTask<ContentDialog> CreateForConfirmCancelAsync(string title, string content, ContentDialogButton defaultButton = ContentDialogButton.Close)
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialog dialog = new()
        {
            XamlRoot = mainWindow.Content.XamlRoot,
            Title = title,
            Content = content,
            DefaultButton = defaultButton,
            PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText,
            CloseButtonText = SH.ContentDialogCancelCloseButtonText,
        };

        return dialog;
    }
}