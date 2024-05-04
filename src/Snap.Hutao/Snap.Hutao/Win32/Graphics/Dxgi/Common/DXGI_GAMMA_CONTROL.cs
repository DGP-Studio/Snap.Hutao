// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi.Common;

internal struct DXGI_GAMMA_CONTROL
{
    public DXGI_RGB Scale;
    public DXGI_RGB Offset;
    public DXGI_RGB_1025 GammaCurve;

    [InlineArray(1025)]
    internal struct DXGI_RGB_1025
    {
        public DXGI_RGB Value;
    }
}