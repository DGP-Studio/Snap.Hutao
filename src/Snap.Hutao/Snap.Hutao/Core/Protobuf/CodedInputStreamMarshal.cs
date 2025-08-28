// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Protobuf;

internal static class CodedInputStreamMarshal
{
    public static bool TryPeekTag(this CodedInputStream stream, out uint tag)
    {
        tag = stream.PeekTag();
        return tag is not 0;
    }

    public static bool TryReadTag(this CodedInputStream stream, out uint tag)
    {
        tag = stream.ReadTag();
        return tag is not 0;
    }

    public static CodedInputStream UnsafeReadLengthDelimitedStream(this CodedInputStream stream)
    {
        return new(ReadRawBytes(stream, stream.ReadLength()));
    }

    // internal byte[] ReadRawBytes(int size)
    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    private static extern byte[] ReadRawBytes(CodedInputStream stream, int size);
}