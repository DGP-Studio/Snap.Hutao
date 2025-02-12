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
    private static readonly Action<Task<ContentDialogResult>, object?> Continuation = RunContinuation;

    private readonly AsyncLock dialogLock = new();

    private readonly ICurrentXamlWindowReference currentWindowReference;
    private readonly ILogger<ContentDialogQueue> logger;
    private readonly ITaskContext taskContext;

    public bool IsDialogShowing
    {
        get
        {
            if (currentWindowReference.Window is null)
            {
                return false;
            }

            if (dialogLock.TryLock(out AsyncLock.Releaser releaser))
            {
                using (releaser)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public ValueContentDialogTask EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog)
    {
        TaskCompletionSource queueSource = new();
        TaskCompletionSource<ContentDialogResult> resultSource = new();

        EnqueueAndShowCoreAsync(contentDialog, queueSource, resultSource).SafeForget(logger);
        return new(queueSource.Task, resultSource.Task);
    }

    private static void RunContinuation(Task<ContentDialogResult> task, object? s)
    {
        ArgumentNullException.ThrowIfNull(s);
        State state = (State)s;
        using (state.Releaser)
        {
            // Mark result as completed when dialog is closed
            state.ResultSource.SetResult(task.Result);
        }
    }

    private async Task EnqueueAndShowCoreAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog, TaskCompletionSource queueSource, TaskCompletionSource<ContentDialogResult> resultSource)
    {
        AsyncLock.Releaser releaser = await dialogLock.LockAsync().ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        queueSource.TrySetResult();
        if (contentDialog.XamlRoot != currentWindowReference.GetXamlRoot())
        {
            // User close the window on previous dialog, and this dialog still using old XamlRoot.
            // And that's why we didn't use dialog's DispatcherQueue to switch thread either.
            HutaoException.NotSupported("Dialog using different XamlRoot");
        }

        State state = new()
        {
            Releaser = releaser,
            ResultSource = resultSource,
        };

        // We need focus the dialog, so we can't await the ShowAsync.
        // ShowAsync returns control when dialog content in VisualTree
        contentDialog
            .ShowAsync()
            .AsTask()
            .ContinueWith(Continuation, state, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)
            .SafeForget(logger);
        contentDialog.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
    }

    private sealed class State
    {
        public required AsyncLock.Releaser Releaser { get; init; }

        public required TaskCompletionSource<ContentDialogResult> ResultSource { get; init; }
    }
}