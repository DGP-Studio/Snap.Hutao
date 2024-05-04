// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.Graphics.Dxgi.Common;

internal struct DXGI_GAMMA_CONTROL_CAPABILITIES
{
    public BOOL ScaleAndOffsetSupported;
    public float MaxConvertedValue;
    public float MinConvertedValue;
    public uint NumGammaControlPoints;
    public unsafe fixed float ControlPointPositions[1025];
}