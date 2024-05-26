// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_CLASS_INSTANCE_DESC
{
    public uint InstanceId;
    public uint InstanceIndex;
    public uint TypeId;
    public uint ConstantBuffer;
    public uint BaseConstantBufferOffset;
    public uint BaseTexture;
    public uint BaseSampler;
    public BOOL Created;
}