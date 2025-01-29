// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Protobuf;

internal static class ByteStringMarshal
{
    public static ByteString Create(ReadOnlyMemory<byte> bytes)
    {
        return CreateByteString(bytes);
    }

    // private ByteString(ReadOnlyMemory<byte> bytes)
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern ByteString CreateByteString(ReadOnlyMemory<byte> bytes);
}