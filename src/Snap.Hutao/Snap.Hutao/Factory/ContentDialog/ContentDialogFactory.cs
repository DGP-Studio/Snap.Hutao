// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Service;

namespace Snap.Hutao.Factory.ContentDialog;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IContentDialogFactory))]
internal sealed partial class ContentDialogFactory : IContentDialogFactory
{
    private readonly ICurrentXamlWindowReference currentWindowReference;
    private readonly IContentDialogQueue contentDialogQueue;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    public bool IsDialogShowing
    {
        get => contentDialogQueue.IsDialogShowing;
    }

    public ITaskContext TaskContext { get => taskContext; }

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
            RequestedTheme = appOptions.ElementTheme,
        };

        return await EnqueueAndShowAsync(dialog).ConfigureAwait(false);
    }

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
            RequestedTheme = appOptions.ElementTheme,
        };

        return await EnqueueAndShowAsync(dialog).ConfigureAwait(false);
    }

    public async ValueTask<Microsoft.UI.Xaml.Controls.ContentDialog> CreateForIndeterminateProgressAsync(string title)
    {
        await taskContext.SwitchToMainThreadAsync();

        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
            Title = title,
            Content = new ProgressBar() { IsIndeterminate = true },
            RequestedTheme = appOptions.ElementTheme,
        };

        return dialog;
    }

    public async ValueTask<TContentDialog> CreateInstanceAsync<TContentDialog>(params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        await taskContext.SwitchToMainThreadAsync();

        TContentDialog contentDialog = ActivatorUtilities.CreateInstance<TContentDialog>(serviceProvider, parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();
        contentDialog.RequestedTheme = appOptions.ElementTheme;

        return contentDialog;
    }

    public TContentDialog CreateInstance<TContentDialog>(params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        TContentDialog contentDialog = ActivatorUtilities.CreateInstance<TContentDialog>(serviceProvider, parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();
        contentDialog.RequestedTheme = appOptions.ElementTheme;

        return contentDialog;
    }

    [SuppressMessage("", "SH003")]
    public Task<ContentDialogResult> EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog, TaskCompletionSource? dialogShowSource = default)
    {
        return contentDialogQueue.EnqueueAndShowAsync(contentDialog, dialogShowSource);
    }
}