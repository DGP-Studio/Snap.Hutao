// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

[SuppressMessage("", "SA1307")]
internal struct DVTARGETDEVICE
{
    public uint tdSize;
    public ushort tdDriverNameOffset;
    public ushort tdDeviceNameOffset;
    public ushort tdPortNameOffset;
    public ushort tdExtDevmodeOffset;
    public FlexibleArray<byte> tdData;
}