// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal static class CodedInputStreamExtensions
{
    public static CodedInputStream ReadLengthDelimitedAsStream(this CodedInputStream stream)
    {
        return new(ReadRawBytes(stream, stream.ReadLength()));
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    private static extern byte[] ReadRawBytes(CodedInputStream stream, int size);
}