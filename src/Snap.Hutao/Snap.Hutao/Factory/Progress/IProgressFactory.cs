// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;

namespace Snap.Hutao.Factory.Progress;

internal interface IProgressFactory
{
    IProgress<T> CreateForMainThread<T>(Action<T> handler);

    IProgress<T> CreateForMainThread<T, TState>([RequireStaticDelegate] Action<T, TState> handler, TState state);
}