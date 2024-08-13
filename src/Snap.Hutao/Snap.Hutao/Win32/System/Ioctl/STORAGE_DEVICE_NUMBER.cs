// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Ioctl;

internal struct STORAGE_DEVICE_NUMBER
{
    public uint DeviceType;
    public uint DeviceNumber;
    public uint PartitionNumber;
}