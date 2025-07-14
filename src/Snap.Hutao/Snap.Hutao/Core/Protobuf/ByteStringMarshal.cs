// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Protobuf;

internal static class ByteStringMarshal
{
    /// <summary>
    /// Creates a new ByteString from the given memory. The memory is <b>not</b>
    /// copied, and must not be modified after this method is called.
    /// </summary>
    /// <param name="bytes">source bytes</param>
    /// <returns>A new ByteString instance</returns>
    public static ByteString Create(ReadOnlyMemory<byte> bytes)
    {
        return CreateByteString(bytes);
    }

    // private ByteString(ReadOnlyMemory<byte> bytes)
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern ByteString CreateByteString(ReadOnlyMemory<byte> bytes);
}