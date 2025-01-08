// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;

namespace Snap.Hutao.Factory.ContentDialog;

[Injection(InjectAs.Singleton, typeof(IContentDialogQueue))]
[SuppressMessage("", "SH003")]
[SuppressMessage("", "SH100")]
[SuppressMessage("", "RS0030")]
[ConstructorGenerated]
internal sealed partial class ContentDialogQueue : IContentDialogQueue
{
    private static readonly Action<Task<ContentDialogResult>, object?> SetContentDialogResult = RunContinuation;

    private readonly AsyncLock dialogLock = new();

    private readonly ICurrentXamlWindowReference currentWindowReference;
    private readonly ILogger<ContentDialogQueue> logger;
    private readonly ITaskContext taskContext;

    public bool IsDialogShowing { get => currentWindowReference.Window is not null && field; private set; }

    public ValueContentDialogTask EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog)
    {
        TaskCompletionSource queueSource = new();
        TaskCompletionSource<ContentDialogResult> resultSource = new();

        EnqueueAndShowCoreAsync(contentDialog, queueSource, resultSource).SafeForget(logger);
        return new(queueSource.Task, resultSource.Task);
    }

    private static void RunContinuation(Task<ContentDialogResult> task, object? state)
    {
        ArgumentNullException.ThrowIfNull(state);
        ((TaskCompletionSource<ContentDialogResult>)state).SetResult(task.Result);
    }

    private async Task EnqueueAndShowCoreAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog, TaskCompletionSource queueSource, TaskCompletionSource<ContentDialogResult> resultSource)
    {
        using (await dialogLock.LockAsync().ConfigureAwait(false))
        {
            await taskContext.SwitchToMainThreadAsync();
            queueSource.TrySetResult();
            if (contentDialog.XamlRoot != currentWindowReference.GetXamlRoot())
            {
                // User close the window on previous dialog, and this dialog still using old XamlRoot.
                // And that's why we didn't use dialog's DispatcherQueue either.
                HutaoException.NotSupported("Dialog using different XamlRoot");
            }

            // We need focus the dialog, so we can't await the ShowAsync.
            contentDialog

                // Return control when dialog content in VisualTree
                .ShowAsync()
                .AsTask()

                // Mark result as completed when dialog is closed
                .ContinueWith(SetContentDialogResult, resultSource, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)
                .SafeForget(logger);
            contentDialog.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        }
    }
}