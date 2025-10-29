// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.LifeCycle;

namespace Snap.Hutao.Factory.ContentDialog;

// It's a view factory
[GeneratedConstructor]
[Service(ServiceLifetime.Singleton, typeof(IContentDialogFactory))]
internal sealed partial class ContentDialogFactory : IContentDialogFactory
{
    private readonly ICurrentXamlWindowReference currentWindowReference;
    private readonly IContentDialogQueue contentDialogQueue;

    public bool IsDialogShowing { get => contentDialogQueue.IsDialogShowing; }

    public partial ITaskContext TaskContext { get; }

    public async ValueTask<ContentDialogResult> CreateForConfirmAsync(string title, string content)
    {
        await TaskContext.SwitchToMainThreadAsync();

        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
            Title = title,
            Content = content,
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText,
            RequestedTheme = currentWindowReference.GetRequestedTheme(),
        };

        return await EnqueueAndShowAsync(dialog).ShowTask.ConfigureAwait(false);
    }

    public async ValueTask<ContentDialogResult> CreateForConfirmCancelAsync(string title, string content, ContentDialogButton defaultButton = ContentDialogButton.Close, bool isPrimaryButtonEnabled = true)
    {
        await TaskContext.SwitchToMainThreadAsync();

        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
            Title = title,
            Content = content,
            DefaultButton = defaultButton,
            PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText,
            CloseButtonText = SH.ContentDialogCancelCloseButtonText,
            IsPrimaryButtonEnabled = isPrimaryButtonEnabled,
            RequestedTheme = currentWindowReference.GetRequestedTheme(),
        };

        return await EnqueueAndShowAsync(dialog).ShowTask.ConfigureAwait(false);
    }

    public async ValueTask<Microsoft.UI.Xaml.Controls.ContentDialog> CreateForIndeterminateProgressAsync(string title)
    {
        await TaskContext.SwitchToMainThreadAsync();

        Microsoft.UI.Xaml.Controls.ContentDialog dialog = new()
        {
            XamlRoot = currentWindowReference.GetXamlRoot(),
            Title = title,
            Content = new ProgressBar { IsIndeterminate = true },
            RequestedTheme = currentWindowReference.GetRequestedTheme(),
        };

        return dialog;
    }

    public async ValueTask<TContentDialog> CreateInstanceAsync<TContentDialog>(IServiceProvider serviceProvider, params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        await TaskContext.SwitchToMainThreadAsync();

        TContentDialog contentDialog = ActivatorUtilities.CreateInstance<TContentDialog>(serviceProvider, parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();
        contentDialog.RequestedTheme = currentWindowReference.GetRequestedTheme();

        return contentDialog;
    }

    public ValueContentDialogTask EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog)
    {
        return contentDialogQueue.EnqueueAndShowAsync(contentDialog);
    }
}