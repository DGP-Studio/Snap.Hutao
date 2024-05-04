// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_COUNTER_INFO
{
    public D3D11_COUNTER LastDeviceDependentCounter;
    public uint NumSimultaneousCounters;
    public byte NumDetectableParallelUnits;
}