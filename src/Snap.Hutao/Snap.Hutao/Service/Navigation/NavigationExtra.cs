// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

internal sealed class NavigationExtra : INavigationData, INavigationAwaiter
{
    private readonly TaskCompletionSource navigationCompletedTaskCompletionSource = new();

    public NavigationExtra(object? data = null)
    {
        Data = data;
    }

    public object? Data { get; set; }

    [SuppressMessage("", "SH003")]
    public Task WaitForCompletionAsync()
    {
        return navigationCompletedTaskCompletionSource.Task;
    }

    public void NotifyNavigationCompleted()
    {
        navigationCompletedTaskCompletionSource.TrySetResult();
    }

    public void NotifyNavigationException(Exception exception)
    {
        navigationCompletedTaskCompletionSource.TrySetException(exception);
    }
}