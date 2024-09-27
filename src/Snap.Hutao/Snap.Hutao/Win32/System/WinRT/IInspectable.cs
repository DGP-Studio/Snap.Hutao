// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.System.WinRT;

internal readonly unsafe struct IInspectable
{
    public readonly global::WinRT.IInspectable.Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xE0, 0xE2, 0x86, 0xAF, 0x2D, 0xB1, 0x6A, 0x4C, 0x9C, 0x5A, 0xD7, 0xAA, 0x65, 0x10, 0x1E, 0x90]);
    }
}