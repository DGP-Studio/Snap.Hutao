// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32.System.Memory;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// NativeMemory.AllocZeroed wrapper
/// </summary>
internal readonly unsafe struct VirtualMemory : IDisposable
{
    /// <summary>
    /// 缓冲区地址
    /// </summary>
    public readonly void* Pointer;

    /// <summary>
    /// 长度
    /// </summary>
    public readonly uint Length;

    /// <summary>
    /// 构造一个新的本地内存
    /// </summary>
    /// <param name="dwSize">长度</param>
    public unsafe VirtualMemory(uint dwSize)
    {
        Length = dwSize;
        VIRTUAL_ALLOCATION_TYPE commitAndReserve = VIRTUAL_ALLOCATION_TYPE.MEM_COMMIT | VIRTUAL_ALLOCATION_TYPE.MEM_RESERVE;
        Pointer = VirtualAlloc(default, dwSize, commitAndReserve, PAGE_PROTECTION_FLAGS.PAGE_READWRITE);
    }

    public static unsafe implicit operator Span<byte>(VirtualMemory memory)
    {
        return memory.GetBuffer();
    }

    /// <summary>
    /// 获取缓冲区
    /// </summary>
    /// <returns>内存</returns>
    public unsafe Span<byte> GetBuffer()
    {
        return new Span<byte>(Pointer, (int)Length);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        VirtualFree(Pointer, 0, VIRTUAL_FREE_TYPE.MEM_RELEASE);
    }
}