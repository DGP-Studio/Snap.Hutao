// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.WinRT;

internal static class IMemoryBufferByteAccessExtension
{
    public static unsafe HRESULT GetBuffer(this IMemoryBufferByteAccess memoryBufferByteAccess, out byte* value, out uint capacity)
    {
        fixed (byte** value2 = &value)
        {
            fixed (uint* capacity2 = &capacity)
            {
                return memoryBufferByteAccess.GetBuffer(value2, capacity2);
            }
        }
    }
}