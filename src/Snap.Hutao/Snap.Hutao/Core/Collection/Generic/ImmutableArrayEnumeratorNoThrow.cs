// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections;
using System.Collections.Immutable;

namespace Snap.Hutao.Core.Collection.Generic;

internal sealed class ImmutableArrayEnumeratorNoThrow<T> : IEnumerator<T>
{
    private readonly ImmutableArray<T> array;
    private int index;

    public ImmutableArrayEnumeratorNoThrow(ImmutableArray<T> array)
    {
        this.array = array;
        index = -1;
    }

    public T Current { get => index < array.Length ? array[index] : default!; }

    object? IEnumerator.Current { get => Current; }

    public bool MoveNext()
    {
        return ++index < array.Length;
    }

    public void Reset()
    {
        index = -1;
    }

    public void Dispose()
    {
    }
}