// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Ioctl;

internal struct DEVICE_TRIM_DESCRIPTOR
{
    public uint Version;
    public uint Size;
    public BOOLEAN TrimEnabled;
}