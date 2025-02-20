// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Com;

[SuppressMessage("", "SA1307")]
internal struct FORMATETC
{
    public ushort cfFormat;
    public unsafe DVTARGETDEVICE* ptd;
    public uint dwAspect;
    public int lindex;
    public uint tymed;
}