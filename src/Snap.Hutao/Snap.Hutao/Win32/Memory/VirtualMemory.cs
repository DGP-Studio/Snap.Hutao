// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Memory;

/// <summary>
/// VirtualAlloc wrapper
/// </summary>
internal readonly unsafe struct VirtualMemory : IUnmanagedMemory
{
    /// <summary>
    /// 缓冲区地址
    /// </summary>
    private readonly void* pointer;

    /// <summary>
    /// 长度
    /// </summary>
    private readonly uint size;

    /// <summary>
    /// 构造一个新的本地内存
    /// </summary>
    /// <param name="dwSize">长度</param>
    public unsafe VirtualMemory(uint dwSize)
    {
        size = dwSize;
        pointer = NativeMemory.Alloc(dwSize);
    }

    /// <inheritdoc/>
    public void* Pointer { get => pointer; }

    /// <inheritdoc/>
    public uint Size { get => size; }

    /// <inheritdoc/>
    public void Dispose()
    {
        NativeMemory.Free(pointer);
    }
}