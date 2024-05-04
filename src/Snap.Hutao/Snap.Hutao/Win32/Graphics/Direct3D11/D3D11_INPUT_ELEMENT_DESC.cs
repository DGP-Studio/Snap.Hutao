// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_INPUT_ELEMENT_DESC
{
    public PCSTR SemanticName;
    public uint SemanticIndex;
    public DXGI_FORMAT Format;
    public uint InputSlot;
    public uint AlignedByteOffset;
    public D3D11_INPUT_CLASSIFICATION InputSlotClass;
    public uint InstanceDataStepRate;
}