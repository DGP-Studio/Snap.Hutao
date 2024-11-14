// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal sealed class LazySlim<T>
{
    private readonly Func<T> valueFactory;

    private bool initialized;
    private object? syncRoot;

    public LazySlim(Func<T> valueFactory)
    {
        this.valueFactory = valueFactory;
    }

    [field: MaybeNull]
    public T Value { get => LazyInitializer.EnsureInitialized(ref field, ref initialized, ref syncRoot, valueFactory); }
}