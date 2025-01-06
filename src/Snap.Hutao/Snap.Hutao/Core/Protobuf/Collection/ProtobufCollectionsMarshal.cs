// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Google.Protobuf.Collections;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Protobuf.Collection;

internal static class ProtobufCollectionsMarshal
{
    public static Span<T> AsSpan<T>(RepeatedField<T> field)
    {
        return new(PrivateGetArray(field), 0, field.Count);
    }

    // private T[] array
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "array")]
    private static extern ref T[] PrivateGetArray<T>(RepeatedField<T> field);
}