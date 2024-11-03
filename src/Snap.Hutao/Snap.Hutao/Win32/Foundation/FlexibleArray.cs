// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Foundation;

internal unsafe struct FlexibleArray<TElement>
    where TElement : unmanaged
{
    public TElement Data;

    [UnscopedRef]
    public ref TElement this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Unsafe.Add(ref Data, index);
    }

    public Span<TElement> AsSpan(int length)
    {
        return MemoryMarshal.CreateSpan(ref Data, length);
    }
}