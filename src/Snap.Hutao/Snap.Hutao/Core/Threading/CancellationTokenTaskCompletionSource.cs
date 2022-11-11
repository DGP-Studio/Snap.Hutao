// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// Holds the task for a cancellation token, as well as the token registration. The registration is disposed when this instance is disposed.
/// </summary>
/// <typeparam name="T">包装类型</typeparam>
public sealed class CancellationTokenTaskCompletionSource : IDisposable
{
    /// <summary>
    /// The cancellation token registration, if any. This is <c>null</c> if the registration was not necessary.
    /// </summary>
    private readonly IDisposable? registration;

    /// <summary>
    /// Creates a task for the specified cancellation token, registering with the token if necessary.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public CancellationTokenTaskCompletionSource(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            Task = Task.CompletedTask;
            return;
        }

        TaskCompletionSource tcs = new TaskCompletionSource();
        registration = cancellationToken.Register(() => tcs.TrySetResult(), useSynchronizationContext: false);
        Task = tcs.Task;
    }

    /// <summary>
    /// Gets the task for the source cancellation token.
    /// </summary>
    public Task Task { get; private set; }

    /// <summary>
    /// Disposes the cancellation token registration, if any. Note that this may cause <see cref="Task"/> to never complete.
    /// </summary>
    public void Dispose()
    {
        registration?.Dispose();
    }
}