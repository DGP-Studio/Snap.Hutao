// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using Snap.Hutao.Core.Protobuf;
using System.Buffers;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

internal readonly struct YaeData : IDisposable
{
    private readonly YaeDataKind kind;
    private readonly IMemoryOwner<byte> owner;
    private readonly int contentLength;

    public YaeData(YaeDataKind kind, IMemoryOwner<byte> owner, int contentLength)
    {
        this.kind = kind;
        this.owner = owner;
        this.contentLength = contentLength;
    }

    public YaeDataKind Kind { get => kind; }

    public ByteString Bytes { get => ByteStringMarshal.Create(owner.Memory[..contentLength]); }

    public void Dispose()
    {
        owner.Dispose();
    }
}