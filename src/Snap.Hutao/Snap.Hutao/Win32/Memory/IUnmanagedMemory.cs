// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Memory;

/// <summary>
/// 非托管内存
/// </summary>
internal unsafe interface IUnmanagedMemory : IDisposable
{
    /// <summary>
    /// Gets a pointer to the allocated unmanaged memory.
    /// </summary>
    void* Pointer { get; }

    /// <summary>
    /// Gets size of referenced unmanaged memory, in bytes.
    /// </summary>
    uint Size { get; }
}