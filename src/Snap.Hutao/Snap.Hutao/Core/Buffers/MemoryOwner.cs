// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using System.Buffers;

namespace Snap.Hutao.Core.Buffers;

internal sealed class MemoryOwner<T> : IMemoryOwner<T>
{
    public static readonly MemoryOwner<T> Empty = new();

    private MemoryOwner()
    {
    }

    public Memory<T> Memory { get => Memory<T>.Empty; }

    public void Dispose()
    {
        // Do nothing, so that can be disposed multiple times safely.
    }
}