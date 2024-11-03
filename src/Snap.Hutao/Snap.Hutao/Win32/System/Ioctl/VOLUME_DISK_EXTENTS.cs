// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Ioctl;

internal struct VOLUME_DISK_EXTENTS
{
    public uint NumberOfDiskExtents;
    public FlexibleArray<DISK_EXTENT> Extents;
}