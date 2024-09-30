// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.Shell;

internal static unsafe class FileOperation
{
    internal static ref readonly Guid CLSID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x75, 0x55, 0xD0, 0x3A, 0x57, 0x88, 0x50, 0x48, 0x92, 0x77, 0x11, 0xB8, 0x5B, 0xDB, 0x8E, 0x09]);
    }
}
