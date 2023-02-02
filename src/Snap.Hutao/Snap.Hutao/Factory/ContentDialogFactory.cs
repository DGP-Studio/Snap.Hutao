// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.Abstraction;

namespace Snap.Hutao.Factory;

/// <inheritdoc cref="IContentDialogFactory"/>
[Injection(InjectAs.Transient, typeof(IContentDialogFactory))]
internal class ContentDialogFactory : IContentDialogFactory
{
    private readonly MainWindow mainWindow;

    /// <summary>
    /// 构造一个新的内容对话框工厂
    /// </summary>
    /// <param name="mainWindow">主窗体</param>
    public ContentDialogFactory(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
    }

    /// <inheritdoc/>
    public async ValueTask<ContentDialogResult> ConfirmAsync(string title, string content)
    {
        ContentDialog dialog = await CreateForConfirmAsync(title, content).ConfigureAwait(false);
        await ThreadHelper.SwitchToMainThreadAsync();
        return await dialog.ShowAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<ContentDialogResult> ConfirmCancelAsync(string title, string content, ContentDialogButton defaultButton = ContentDialogButton.Close)
    {
        ContentDialog dialog = await CreateForConfirmCancelAsync(title, content, defaultButton).ConfigureAwait(false);
        await ThreadHelper.SwitchToMainThreadAsync();
        return await dialog.ShowAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<ContentDialog> CreateForIndeterminateProgressAsync(string title)
    {
        await ThreadHelper.SwitchToMainThreadAsync();
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
        await ThreadHelper.SwitchToMainThreadAsync();
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
        await ThreadHelper.SwitchToMainThreadAsync();
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