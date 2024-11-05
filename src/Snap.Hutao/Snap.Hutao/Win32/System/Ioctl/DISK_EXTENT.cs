// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Ioctl;

internal struct DISK_EXTENT
{
    public uint DiskNumber;
    public long StartingOffset;
    public long ExtentLength;
}