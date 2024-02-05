// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 任务上下文
/// </summary>
internal interface ITaskContext
{
    void BeginInvokeOnMainThread(Action action);

    void InvokeOnMainThread(Action action);

    T InvokeOnMainThread<T>(Func<T> action);

    ThreadPoolSwitchOperation SwitchToBackgroundAsync();

    DispatcherQueueSwitchOperation SwitchToMainThreadAsync();
}