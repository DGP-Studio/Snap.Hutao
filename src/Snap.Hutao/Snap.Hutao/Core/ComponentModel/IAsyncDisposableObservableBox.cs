// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.ComponentModel;

internal interface IAsyncDisposableObservableBox<out T> : IAsyncDisposable
    where T : class?, IDisposable?
{
    T Value { get; }

    AsyncLock SyncRoot { get; }
}