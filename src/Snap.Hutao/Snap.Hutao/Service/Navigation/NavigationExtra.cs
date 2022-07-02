// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

/// <summary>
/// 导航额外信息
/// </summary>
public class NavigationExtra : INavigationExtra, INavigationAwaiter
{
    /// <summary>
    /// 任务完成源
    /// </summary>
    private readonly TaskCompletionSource navigationCompletedTaskCompletionSource = new();

    /// <summary>
    /// 构造一个新的导航额外信息
    /// </summary>
    /// <param name="data">数据</param>
    public NavigationExtra(object? data = null)
    {
        Data = data;
    }

    /// <inheritdoc/>
    public object? Data { get; set; }

    /// <inheritdoc/>
    public Task WaitForCompletionAsync()
    {
        return navigationCompletedTaskCompletionSource.Task;
    }

    /// <inheritdoc/>
    public void NotifyNavigationCompleted()
    {
        navigationCompletedTaskCompletionSource.TrySetResult();
    }

    /// <inheritdoc/>
    public void NotifyNavigationException(Exception exception)
    {
        navigationCompletedTaskCompletionSource.TrySetException(exception);
    }
}