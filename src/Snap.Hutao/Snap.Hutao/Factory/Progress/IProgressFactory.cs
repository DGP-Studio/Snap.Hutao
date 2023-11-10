// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Factory.Progress;

internal interface IProgressFactory
{
    IProgress<T> CreateForMainThread<T>(Action<T> handler);
}