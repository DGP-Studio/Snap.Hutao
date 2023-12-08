// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 任务上下文
/// </summary>
internal interface ITaskContext
{
    SynchronizationContext SynchronizationContext { get; }

    void BeginInvokeOnMainThread(Action action);

    void InvokeOnMainThread(Action action);

    ThreadPoolSwitchOperation SwitchToBackgroundAsync();

    DispatcherQueueSwitchOperation SwitchToMainThreadAsync();
}