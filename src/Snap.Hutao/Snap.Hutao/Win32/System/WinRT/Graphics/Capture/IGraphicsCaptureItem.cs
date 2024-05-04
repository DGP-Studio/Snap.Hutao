// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.System.WinRT.Graphics.Capture;

internal unsafe struct IGraphicsCaptureItem
{
    internal static ref readonly Guid IID
    {
        get
        {
            // 79C3F95B-31F7-4EC2-A464-632EF5D30760
            ReadOnlySpan<byte> data = [0x5B, 0xF9, 0xC3, 0x79, 0xF7, 0x31, 0xC2, 0x4E, 0xA4, 0x64, 0x63, 0x2E, 0xF5, 0xD3, 0x07, 0x60];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }
}
