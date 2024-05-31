// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal struct DXGI_SWAP_CHAIN_DESC
{
    public DXGI_MODE_DESC BufferDesc;
    public DXGI_SAMPLE_DESC SampleDesc;
    public DXGI_USAGE BufferUsage;
    public uint BufferCount;
    public HWND OutputWindow;
    public BOOL Windowed;
    public DXGI_SWAP_EFFECT SwapEffect;
    public uint Flags;
}