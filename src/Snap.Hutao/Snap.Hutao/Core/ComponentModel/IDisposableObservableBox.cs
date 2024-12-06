// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.ComponentModel;

internal interface IDisposableObservableBox<out T> : IDisposable
    where T : class?, IDisposable?
{
    T Value { get; }
}