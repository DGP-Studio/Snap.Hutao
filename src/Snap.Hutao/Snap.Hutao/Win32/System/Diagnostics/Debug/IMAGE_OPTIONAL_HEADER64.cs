// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.System.Diagnostics.Debug;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct IMAGE_OPTIONAL_HEADER64
{
    public IMAGE_OPTIONAL_HEADER_MAGIC Magic;
    public byte MajorLinkerVersion;
    public byte MinorLinkerVersion;
    public uint SizeOfCode;
    public uint SizeOfInitializedData;
    public uint SizeOfUninitializedData;
    public uint AddressOfEntryPoint;
    public uint BaseOfCode;
    public ulong ImageBase;
    public uint SectionAlignment;
    public uint FileAlignment;
    public ushort MajorOperatingSystemVersion;
    public ushort MinorOperatingSystemVersion;
    public ushort MajorImageVersion;
    public ushort MinorImageVersion;
    public ushort MajorSubsystemVersion;
    public ushort MinorSubsystemVersion;
    public uint Win32VersionValue;
    public uint SizeOfImage;
    public uint SizeOfHeaders;
    public uint CheckSum;
    public IMAGE_SUBSYSTEM Subsystem;
    public IMAGE_DLL_CHARACTERISTICS DllCharacteristics;
    public ulong SizeOfStackReserve;
    public ulong SizeOfStackCommit;
    public ulong SizeOfHeapReserve;
    public ulong SizeOfHeapCommit;

    [Obsolete]
    public uint LoaderFlags;
    public uint NumberOfRvaAndSizes;
    public IMAGE_DATA_DIRECTORY_16 DataDirectory;

    [InlineArray(16)]
    internal struct IMAGE_DATA_DIRECTORY_16
    {
        public IMAGE_DATA_DIRECTORY Value;
    }
}