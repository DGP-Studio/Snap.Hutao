// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Storage.FileSystem;

namespace Snap.Hutao.Win32.System.Ioctl;

internal struct STORAGE_DEVICE_DESCRIPTOR
{
    public uint Version;

    public uint Size;

    public byte DeviceType;

    public byte DeviceTypeModifier;

    public BOOLEAN RemovableMedia;

    public BOOLEAN CommandQueueing;

    public uint VendorIdOffset;

    public uint ProductIdOffset;

    public uint ProductRevisionOffset;

    public uint SerialNumberOffset;

    public STORAGE_BUS_TYPE BusType;

    public uint RawPropertiesLength;

    public FlexibleArray<byte> RawDeviceProperties;
}