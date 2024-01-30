// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.Shell;

[Guid("C0B4E2F3-BA21-4773-8DBA-335EC946EB8B")]
internal readonly struct FileSaveDialog
{
    internal static unsafe ref readonly Guid CLSID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xF3, 0xE2, 0xB4, 0xC0, 0x21, 0xBA, 0x73, 0x47, 0x8D, 0xBA, 0x33, 0x5E, 0xC9, 0x46, 0xEB, 0x8B];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }
}