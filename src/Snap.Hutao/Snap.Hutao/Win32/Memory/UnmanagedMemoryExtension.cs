// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Memory;

internal static class UnmanagedMemoryExtension
{
    public static unsafe Span<byte> AsSpan(this IUnmanagedMemory unmanagedMemory)
    {
        return new(unmanagedMemory.Pointer, (int)unmanagedMemory.Size);
    }
}