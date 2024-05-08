// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core;

internal readonly ref struct ReadOnlySpan2D<T>
    where T : unmanaged
{
    private readonly ref T reference;
    private readonly int length;
    private readonly int columns;

    public unsafe ReadOnlySpan2D(void* pointer, int length, int columns)
    {
        reference = ref *(T*)pointer;
        this.length = length;
        this.columns = columns;
    }

    public ReadOnlySpan<T> this[int row]
    {
        get => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref reference, row * columns), columns);
    }
}