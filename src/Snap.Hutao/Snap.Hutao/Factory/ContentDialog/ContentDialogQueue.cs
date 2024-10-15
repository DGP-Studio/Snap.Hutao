// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.LifeCycle;
using System.Collections.Concurrent;

namespace Snap.Hutao.Factory.ContentDialog;

[Injection(InjectAs.Singleton, typeof(IContentDialogQueue))]
[SuppressMessage("", "SH003")]
[ConstructorGenerated]
internal sealed partial class ContentDialogQueue : IContentDialogQueue
{
    private static readonly Action<Task<ContentDialogResult>, object?> Continuation = RunContinuation;

    private readonly ConcurrentQueue<Func<Task>> dialogQueue = [];

    private readonly ICurrentXamlWindowReference currentWindowReference;
    private readonly ILogger<ContentDialogQueue> logger;
    private readonly ITaskContext taskContext;

    private bool isDialogShowing;

    public bool IsDialogShowing { get => currentWindowReference.Window is not null && isDialogShowing; }

    [SuppressMessage("", "SH100")]
    public Task<ContentDialogResult> EnqueueAndShowAsync(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog, TaskCompletionSource? dialogShowSource = default)
    {
        TaskCompletionSource<ContentDialogResult> dialogResultSource = new();

        dialogQueue.Enqueue(async () =>
        {
            try
            {
                await taskContext.SwitchToMainThreadAsync();
                dialogShowSource?.TrySetResult();
                contentDialog.ShowAsync().AsTask()
                    .ContinueWith(Continuation, dialogResultSource, default, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)
                    .SafeForget(logger);
                contentDialog.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            }
            catch (Exception ex)
            {
                dialogResultSource.SetException(ex);
            }
            finally
            {
                ShowNextDialog().SafeForget(logger);
            }
        });

        if (!isDialogShowing)
        {
            ShowNextDialog();
        }

        return dialogResultSource.Task;
    }

    private static void RunContinuation(Task<ContentDialogResult> task, object? state)
    {
        ArgumentNullException.ThrowIfNull(state);
        ((TaskCompletionSource<ContentDialogResult>)state).SetResult(task.Result);
    }

    private Task ShowNextDialog()
    {
        if (dialogQueue.TryDequeue(out Func<Task>? showNextDialogAsync))
        {
            isDialogShowing = true;
            return showNextDialogAsync();
        }

        isDialogShowing = false;
        return Task.CompletedTask;
    }
}