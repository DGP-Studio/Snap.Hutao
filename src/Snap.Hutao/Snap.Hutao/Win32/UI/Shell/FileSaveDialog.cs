// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.Shell;

internal static unsafe class FileSaveDialog
{
    internal static ref readonly Guid CLSID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xF3, 0xE2, 0xB4, 0xC0, 0x21, 0xBA, 0x73, 0x47, 0x8D, 0xBA, 0x33, 0x5E, 0xC9, 0x46, 0xEB, 0x8B]);
    }
}