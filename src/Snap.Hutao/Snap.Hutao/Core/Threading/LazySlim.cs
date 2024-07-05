// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal sealed class LazySlim<T>
{
    private readonly Func<T> valueFactory;

    [MaybeNull]
    private T value;
    private bool initialized;
    private object? syncRoot;

    public LazySlim(Func<T> valueFactory)
    {
        this.valueFactory = valueFactory;
    }

    public T Value { get => LazyInitializer.EnsureInitialized(ref value, ref initialized, ref syncRoot, valueFactory); }
}