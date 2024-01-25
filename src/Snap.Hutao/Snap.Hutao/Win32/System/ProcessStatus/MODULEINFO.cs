// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.ProcessStatus;

[SuppressMessage("", "SA1307")]
internal struct MODULEINFO
{
    public unsafe void* lpBaseOfDll;
    public uint SizeOfImage;
    public unsafe void* EntryPoint;
}