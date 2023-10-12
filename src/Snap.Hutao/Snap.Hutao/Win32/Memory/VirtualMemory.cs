// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32.System.Memory;
using static Windows.Win32.PInvoke;

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
        VIRTUAL_ALLOCATION_TYPE commitAndReserve = VIRTUAL_ALLOCATION_TYPE.MEM_COMMIT | VIRTUAL_ALLOCATION_TYPE.MEM_RESERVE;
        pointer = VirtualAlloc(default, dwSize, commitAndReserve, PAGE_PROTECTION_FLAGS.PAGE_READWRITE);
    }

    /// <inheritdoc/>
    public void* Pointer { get => pointer; }

    /// <inheritdoc/>
    public uint Size { get => size; }

    /// <inheritdoc/>
    public void Dispose()
    {
        VirtualFree(pointer, 0, VIRTUAL_FREE_TYPE.MEM_RELEASE);
    }
}