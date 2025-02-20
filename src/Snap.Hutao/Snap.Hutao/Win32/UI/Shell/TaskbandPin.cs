// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.Shell;

internal static class TaskbandPin
{
    internal static ref readonly Guid CLSID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x4E, 0x3A, 0xAA, 0x90, 0xBA, 0x1C, 0x33, 0x42, 0xB8, 0xBB, 0x53, 0x57, 0x73, 0xD4, 0x84, 0x49]);
    }
}