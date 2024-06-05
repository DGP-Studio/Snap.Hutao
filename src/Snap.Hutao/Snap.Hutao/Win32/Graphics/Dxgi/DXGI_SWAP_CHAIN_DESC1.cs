// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal struct DXGI_SWAP_CHAIN_DESC1
{
    public uint Width;
    public uint Height;
    public DXGI_FORMAT Format;
    public BOOL Stereo;
    public DXGI_SAMPLE_DESC SampleDesc;
    public DXGI_USAGE BufferUsage;
    public uint BufferCount;
    public DXGI_SCALING Scaling;
    public DXGI_SWAP_EFFECT SwapEffect;
    public DXGI_ALPHA_MODE AlphaMode;
    public DXGI_SWAP_CHAIN_FLAG Flags;
}