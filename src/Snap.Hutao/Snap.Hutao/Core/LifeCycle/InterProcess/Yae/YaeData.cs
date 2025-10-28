// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using Snap.Hutao.Core.Protobuf;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

internal sealed partial class YaeData : IDisposable
{
    private readonly IMemoryOwner<byte> owner;
    private readonly int contentLength;

    public YaeData(YaeCommandKind kind, IMemoryOwner<byte> owner, int contentLength)
    {
        Kind = kind;
        this.owner = owner;
        this.contentLength = contentLength;
    }

    ~YaeData()
    {
        Dispose();
    }

    public static YaeData SessionEnd { get => new(YaeCommandKind.SessionEnd, IMemoryOwner<byte>.Empty, 0); }

    public YaeCommandKind Kind { get; }

    public ByteString Bytes { get => ByteStringMarshal.Create(owner.Memory[..contentLength]); }

    public ref readonly YaePropertyTypeValue PropertyTypeValue
    {
        get => ref MemoryMarshal.AsRef<YaePropertyTypeValue>(owner.Memory.Span[..contentLength]);
    }

    public void Dispose()
    {
        owner.Dispose();
        GC.SuppressFinalize(this);
    }
}