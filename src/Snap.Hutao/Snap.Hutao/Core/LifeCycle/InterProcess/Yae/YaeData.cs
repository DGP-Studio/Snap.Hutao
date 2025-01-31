// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using Snap.Hutao.Core.Protobuf;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

internal sealed class YaeData : IDisposable
{
    private readonly IMemoryOwner<byte> owner;
    private readonly int contentLength;

    public YaeData(YaeDataKind kind, IMemoryOwner<byte> owner, int contentLength)
    {
        Kind = kind;
        this.owner = owner;
        this.contentLength = contentLength;
    }

    ~YaeData()
    {
        Dispose();
    }

    public YaeDataKind Kind { get; }

    public ReadOnlyMemory<byte> Memory { get => owner.Memory[..contentLength]; }

    public ByteString Bytes { get => ByteStringMarshal.Create(owner.Memory[..contentLength]); }

    public ref readonly YaePropertyTypeValue PropertyTypeValue
    {
        get => ref MemoryMarshal.AsRef<YaePropertyTypeValue>((ReadOnlySpan<byte>)owner.Memory.Span[..contentLength]);
    }

    public void Dispose()
    {
        owner.Dispose();
        GC.SuppressFinalize(this);
    }
}