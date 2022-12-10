// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.WinRT;

/// <summary>
/// Provides access to an IMemoryBuffer as an array of bytes.
/// </summary>
[ComImport]
[Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public unsafe interface IMemoryBufferByteAccess
{
    /// <summary>
    /// Gets an IMemoryBuffer as an array of bytes.
    /// </summary>
    /// <param name="buffer">指向包含缓冲区数据的字节数组的指针</param>
    /// <param name="capacity">返回数组中的字节数</param>
    void GetBuffer(out byte* buffer, out uint capacity);
}
