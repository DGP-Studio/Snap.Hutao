// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;

namespace Snap.Hutao.Core.Threading;

internal interface ITaskContext : ITaskContextUnsafe
{
    void BeginInvokeOnMainThread([RequireStaticDelegate] Action action);

    void InvokeOnMainThread([InstantHandle] Action action);

    T InvokeOnMainThread<T>([InstantHandle] Func<T> func);

    ThreadPoolSwitchOperation SwitchToBackgroundAsync();

    DispatcherQueueSwitchOperation SwitchToMainThreadAsync();
}