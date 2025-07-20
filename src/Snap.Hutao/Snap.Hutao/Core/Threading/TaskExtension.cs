// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.Core.Threading;

internal static class TaskExtension
{
    public static async void SafeForget(this Task task)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception ex)
        {
            ExceptionHandlingSupport.KillProcessOnDbExceptionNoThrow(ex);
            ex.SetSentryMechanism("TaskExtension.SafeForget", handled: true);
            SentrySdk.CaptureException(ex);
        }
    }

    public static async void SafeForget(this ValueTask task)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception ex)
        {
            ExceptionHandlingSupport.KillProcessOnDbExceptionNoThrow(ex);
            ex.SetSentryMechanism("TaskExtension.SafeForget", handled: true);
            SentrySdk.CaptureException(ex);
        }
    }

    [SuppressMessage("", "SH003")]
    public static async Task WhenAllOrAnyException(params Task[] tasks)
    {
        using (CancellationTokenSource taskFaultedCts = new())
        {
            // ReSharper disable once AccessToDisposedClosure
            List<Task> taskList = [.. tasks.Select(task => WrapTask(task, taskFaultedCts.Token))];

            Task firstCompletedTask = await Task.WhenAny(taskList).ConfigureAwait(true);

            if (firstCompletedTask.IsFaulted)
            {
#pragma warning disable CA1849
                // ReSharper disable once MethodHasAsyncOverload
                taskFaultedCts.Cancel();
#pragma warning restore CA1849
                await firstCompletedTask.ConfigureAwait(true);
            }

            await Task.WhenAll(taskList).ConfigureAwait(true);
        }

        static async Task WrapTask(Task task, CancellationToken token)
        {
            try
            {
                await task.ConfigureAwait(true);
            }
            catch (Exception) when (!token.IsCancellationRequested)
            {
                throw;
            }
        }
    }
}