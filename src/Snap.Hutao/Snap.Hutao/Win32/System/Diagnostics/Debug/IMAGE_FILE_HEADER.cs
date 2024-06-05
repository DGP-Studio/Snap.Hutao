// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.System.SystemInformation;

namespace Snap.Hutao.Win32.System.Diagnostics.Debug;

internal struct IMAGE_FILE_HEADER
{
    public IMAGE_FILE_MACHINE Machine;
    public ushort NumberOfSections;
    public uint TimeDateStamp;
    public uint PointerToSymbolTable;
    public uint NumberOfSymbols;
    public ushort SizeOfOptionalHeader;
    public IMAGE_FILE_CHARACTERISTICS Characteristics;
}