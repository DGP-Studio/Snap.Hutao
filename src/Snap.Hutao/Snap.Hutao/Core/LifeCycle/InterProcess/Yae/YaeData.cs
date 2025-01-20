// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using Snap.Hutao.Core.Protobuf;
using System.Buffers;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

internal readonly struct YaeData : IDisposable
{
    private readonly YaePacketHeader header;
    private readonly IMemoryOwner<byte> owner;

    public YaeData(YaePacketHeader header, IMemoryOwner<byte> owner)
    {
        this.header = header;
        this.owner = owner;
    }

    public YaeDataKind Kind { get => header.Kind; }

    public ByteString Bytes { get => ByteStringMarshal.Create(owner.Memory[..header.ContentLength]); }

    public void Dispose()
    {
        owner.Dispose();
    }
}