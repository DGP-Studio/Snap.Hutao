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

    public bool IsDialogShowing
    {
        get => contentDialogQueue.IsDialogShowing;
    }

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
        };

        return await EnqueueAndShowAsync(dialog).ShowTask.ConfigureAwait(false);
    }

    public async ValueTask<ContentDialogResult> CreateForConfirmCancelAsync(string title, string content, ContentDialogButton defaultButton = ContentDialogButton.Close)
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
        };

        return dialog;
    }

    public async ValueTask<TContentDialog> CreateInstanceAsync<TContentDialog>(params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        await TaskContext.SwitchToMainThreadAsync();

        TContentDialog contentDialog = ActivatorUtilities.CreateInstance<TContentDialog>(serviceProvider, parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();

        return contentDialog;
    }

    public ValueContentDialogTask EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog)
    {
        return contentDialogQueue.EnqueueAndShowAsync(contentDialog);
    }
}