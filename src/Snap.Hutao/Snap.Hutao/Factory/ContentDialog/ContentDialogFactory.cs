// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Service;

namespace Snap.Hutao.Factory.ContentDialog;

/// <inheritdoc cref="IContentDialogFactory"/>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IContentDialogFactory))]
internal sealed partial class ContentDialogFactory : IContentDialogFactory
{
    private readonly ICurrentXamlWindowReference currentWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    private Microsoft.UI.Xaml.Controls.ContentDialog? currentContentDialog;

    /// <inheritdoc/>
    public async ValueTask<ContentDialogResult> CreateForConfirmAsync(string title, string content)
    {
        await HideAllDialogsAsync().ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();

        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
            Title = title,
            Content = content,
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText,
            RequestedTheme = appOptions.ElementTheme,
        };

        dialog.Closed += OnContentDialogClosed;
        currentContentDialog = dialog;
        return await dialog.ShowAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<ContentDialogResult> CreateForConfirmCancelAsync(string title, string content, ContentDialogButton defaultButton = ContentDialogButton.Close)
    {
        await HideAllDialogsAsync().ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();

        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
            Title = title,
            Content = content,
            DefaultButton = defaultButton,
            PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText,
            CloseButtonText = SH.ContentDialogCancelCloseButtonText,
            RequestedTheme = appOptions.ElementTheme,
        };

        dialog.Closed += OnContentDialogClosed;
        currentContentDialog = dialog;
        return await dialog.ShowAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<Microsoft.UI.Xaml.Controls.ContentDialog> CreateForIndeterminateProgressAsync(string title)
    {
        await HideAllDialogsAsync().ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();

        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
            Title = title,
            Content = new ProgressBar() { IsIndeterminate = true },
            RequestedTheme = appOptions.ElementTheme,
        };

        dialog.Closed += OnContentDialogClosed;
        currentContentDialog = dialog;
        return dialog;
    }

    public async ValueTask<TContentDialog> CreateInstanceAsync<TContentDialog>(params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        await HideAllDialogsAsync().ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();

        TContentDialog contentDialog = serviceProvider.CreateInstance<TContentDialog>(parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();
        contentDialog.RequestedTheme = appOptions.ElementTheme;

        contentDialog.Closed += OnContentDialogClosed;
        currentContentDialog = contentDialog;
        return contentDialog;
    }

    public TContentDialog CreateInstance<TContentDialog>(params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        HideAllDialogs();

        TContentDialog contentDialog = serviceProvider.CreateInstance<TContentDialog>(parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();
        contentDialog.RequestedTheme = appOptions.ElementTheme;

        contentDialog.Closed += OnContentDialogClosed;
        currentContentDialog = contentDialog;
        return contentDialog;
    }

    public void HideAllDialogs()
    {
        currentContentDialog?.Hide();
    }

    public async ValueTask HideAllDialogsAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        currentContentDialog?.Hide();
    }

    private void OnContentDialogClosed(Microsoft.UI.Xaml.Controls.ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        currentContentDialog = null;
        sender.Closed -= OnContentDialogClosed;
    }
}