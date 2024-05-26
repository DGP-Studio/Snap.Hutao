// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Graphics.Dxgi.Common;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal struct DXGI_SURFACE_DESC
{
    public uint Width;
    public uint Height;
    public DXGI_FORMAT Format;
    public DXGI_SAMPLE_DESC SampleDesc;
}