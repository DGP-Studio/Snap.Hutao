// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
            ex.SetSentryMechanism("TaskExtension.SafeForget", handled: true);
            SentrySdk.CaptureException(ex);
        }
    }

    [SuppressMessage("", "SH003")]
    public static async Task WhenAllOrAnyException(params Task[] tasks)
    {
        using (CancellationTokenSource taskFaultedCts = new())
        {
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

    public static async Task SuppressThrowing<T>(this Task task)
        where T : Exception
    {
        try
        {
            await task;
        }
        catch (T)
        {
        }
    }
}