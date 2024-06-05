// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal struct DXGI_ADAPTER_DESC
{
    public unsafe fixed char Description[128];
    public uint VendorId;
    public uint DeviceId;
    public uint SubSysId;
    public uint Revision;
    public nuint DedicatedVideoMemory;
    public nuint DedicatedSystemMemory;
    public nuint SharedSystemMemory;
    public LUID AdapterLuid;
}