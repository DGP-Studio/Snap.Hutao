// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.LifeCycle;

namespace Snap.Hutao.Factory.ContentDialog;

/// <inheritdoc cref="IContentDialogFactory"/>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IContentDialogFactory))]
internal sealed partial class ContentDialogFactory : IContentDialogFactory
{
    private readonly ICurrentWindowReference currentWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<ContentDialogResult> CreateForConfirmAsync(string title, string content)
    {
        await taskContext.SwitchToMainThreadAsync();
        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
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
        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
            Title = title,
            Content = content,
            DefaultButton = defaultButton,
            PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText,
            CloseButtonText = SH.ContentDialogCancelCloseButtonText,
        };

        return await dialog.ShowAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<Microsoft.UI.Xaml.Controls.ContentDialog> CreateForIndeterminateProgressAsync(string title)
    {
        await taskContext.SwitchToMainThreadAsync();
        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
            Title = title,
            Content = new ProgressBar() { IsIndeterminate = true },
        };

        return dialog;
    }

    public async ValueTask<TContentDialog> CreateInstanceAsync<TContentDialog>(params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        await taskContext.SwitchToMainThreadAsync();
        TContentDialog contentDialog = serviceProvider.CreateInstance<TContentDialog>(parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();
        return contentDialog;
    }

    public TContentDialog CreateInstance<TContentDialog>(params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        TContentDialog contentDialog = serviceProvider.CreateInstance<TContentDialog>(parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();
        return contentDialog;
    }
}