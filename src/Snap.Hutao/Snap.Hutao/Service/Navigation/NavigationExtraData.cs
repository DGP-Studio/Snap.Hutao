// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Animation;

namespace Snap.Hutao.Service.Navigation;

internal class NavigationExtraData : INavigationExtraData, ISupportNavigationTransitionInfo
{
    private readonly TaskCompletionSource navigationCompletedTcs = new();

    public NavigationExtraData(object? data = null)
    {
        Data = data;
    }

    public NavigationExtraData(NavigationTransitionInfo transitionInfo, object? data = null)
    {
        TransitionInfo = transitionInfo;
        Data = data;
    }

    public static NavigationExtraData Default { get => new(); }

    public object? Data { get; set; }

    public NavigationTransitionInfo? TransitionInfo { get; }

    [SuppressMessage("", "SH003")]
    public Task WaitForCompletionAsync()
    {
        return navigationCompletedTcs.Task;
    }

    public void NotifyNavigationCompleted()
    {
        navigationCompletedTcs.TrySetResult();
    }

    public void NotifyNavigationException(Exception exception)
    {
        navigationCompletedTcs.TrySetException(exception);
    }
}

[SuppressMessage("", "SA1402")]
internal class NavigationExtraData<T> : NavigationExtraData
    where T : class
{
    public NavigationExtraData(NavigationTransitionInfo transitionInfo, T data)
        : base(transitionInfo, data)
    {
    }

    public NavigationExtraData(T data)
        : base(data)
    {
    }

    public T? TypedData { get => Data as T; set => Data = value; }
}