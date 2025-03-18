// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;

namespace Snap.Hutao.Core.Threading;

internal interface ITaskContext
{
    void BeginInvokeOnMainThread(Action action);

    void InvokeOnMainThread(Action action);

    T InvokeOnMainThread<T>([InstantHandle] Func<T> func);

    ThreadPoolSwitchOperation SwitchToBackgroundAsync();

    DispatcherQueueSwitchOperation SwitchToMainThreadAsync();
}