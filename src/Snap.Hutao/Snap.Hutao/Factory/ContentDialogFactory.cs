// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Factory.Abstraction;

namespace Snap.Hutao.Factory;

/// <inheritdoc cref="IContentDialogFactory"/>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IContentDialogFactory))]
internal sealed partial class ContentDialogFactory : IContentDialogFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly ICurrentWindowReference currentWindowReference;

    /// <inheritdoc/>
    public async ValueTask<ContentDialogResult> CreateForConfirmAsync(string title, string content)
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.Window.Content.XamlRoot,
            Title = title,
            Content = content,
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText,
        };

        return await dialog.ShowAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<ContentDialogResult> CreateForConfirmCancelAsync(string title, string content, ContentDialogButton defaultButton = ContentDialogButton.Close)
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.Window.Content.XamlRoot,
            Title = title,
            Content = content,
            DefaultButton = defaultButton,
            PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText,
            CloseButtonText = SH.ContentDialogCancelCloseButtonText,
        };

        return await dialog.ShowAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<ContentDialog> CreateForIndeterminateProgressAsync(string title)
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.Window.Content.XamlRoot,
            Title = title,
            Content = new ProgressBar() { IsIndeterminate = true },
        };

        return dialog;
    }

    public async ValueTask<TContentDialog> CreateInstanceAsync<TContentDialog>(params object[] parameters)
        where TContentDialog : ContentDialog
    {
        await taskContext.SwitchToMainThreadAsync();
        TContentDialog contentDialog = serviceProvider.CreateInstance<TContentDialog>(parameters);
        contentDialog.XamlRoot = currentWindowReference.Window.Content.XamlRoot;
        return contentDialog;
    }

    public TContentDialog CreateInstance<TContentDialog>(params object[] parameters)
        where TContentDialog : ContentDialog
    {
        TContentDialog contentDialog = serviceProvider.CreateInstance<TContentDialog>(parameters);
        contentDialog.XamlRoot = currentWindowReference.Window.Content.XamlRoot;
        return contentDialog;
    }
}