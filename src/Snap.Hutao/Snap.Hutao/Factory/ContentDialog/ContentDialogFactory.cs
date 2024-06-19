// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Service;
using System.Collections.Concurrent;

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

    private readonly ConcurrentQueue<Func<Task>> dialogQueue = [];
    private bool isDialogShowing;

    public bool IsDialogShowing
    {
        get
        {
            if (currentWindowReference.Window is not { } window)
            {
                return false;
            }

            return isDialogShowing;
        }
    }

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
            RequestedTheme = appOptions.ElementTheme,
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
            RequestedTheme = appOptions.ElementTheme,
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
            RequestedTheme = appOptions.ElementTheme,
        };

        return dialog;
    }

    public async ValueTask<TContentDialog> CreateInstanceAsync<TContentDialog>(params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        await taskContext.SwitchToMainThreadAsync();

        TContentDialog contentDialog = serviceProvider.CreateInstance<TContentDialog>(parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();
        contentDialog.RequestedTheme = appOptions.ElementTheme;

        return contentDialog;
    }

    public TContentDialog CreateInstance<TContentDialog>(params object[] parameters)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        TContentDialog contentDialog = serviceProvider.CreateInstance<TContentDialog>(parameters);
        contentDialog.XamlRoot = currentWindowReference.GetXamlRoot();
        contentDialog.RequestedTheme = appOptions.ElementTheme;

        return contentDialog;
    }

    [SuppressMessage("", "SH003")]
    public Task<ContentDialogResult> EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog)
    {
        TaskCompletionSource<ContentDialogResult> dialogShowCompletionSource = new();

        dialogQueue.Enqueue(async () =>
        {
            try
            {
                ContentDialogResult result = await contentDialog.ShowAsync();
                dialogShowCompletionSource.SetResult(result);
            }
            catch (Exception ex)
            {
                dialogShowCompletionSource.SetException(ex);
            }
            finally
            {
                ShowNextDialog().SafeForget();
            }
        });

        if (!isDialogShowing)
        {
            ShowNextDialog();
        }

        return dialogShowCompletionSource.Task;

        Task ShowNextDialog()
        {
            if (dialogQueue.TryDequeue(out Func<Task>? showNextDialogAsync))
            {
                isDialogShowing = true;
                return showNextDialogAsync();
            }
            else
            {
                isDialogShowing = false;
                return Task.CompletedTask;
            }
        }
    }
}