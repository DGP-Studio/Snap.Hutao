// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_DEPTH_STENCILOP_DESC
{
    public D3D11_STENCIL_OP StencilFailOp;
    public D3D11_STENCIL_OP StencilDepthFailOp;
    public D3D11_STENCIL_OP StencilPassOp;
    public D3D11_COMPARISON_FUNC StencilFunc;
}