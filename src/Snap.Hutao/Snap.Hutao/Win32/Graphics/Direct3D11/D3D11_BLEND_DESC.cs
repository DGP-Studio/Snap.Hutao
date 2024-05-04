// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_BLEND_DESC
{
    public BOOL AlphaToCoverageEnable;
    public BOOL IndependentBlendEnable;
    public D3D11_RENDER_TARGET_BLEND_DESC_8 RenderTarget;

    [InlineArray(8)]
    internal struct D3D11_RENDER_TARGET_BLEND_DESC_8
    {
        public D3D11_RENDER_TARGET_BLEND_DESC Value;
    }
}