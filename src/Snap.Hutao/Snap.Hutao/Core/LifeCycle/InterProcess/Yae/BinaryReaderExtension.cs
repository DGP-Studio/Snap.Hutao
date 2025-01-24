// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

internal static class BinaryReaderExtension
{
    public static unsafe T Read<T>(this BinaryReader reader)
        where T : unmanaged
    {
        T data = default;
        reader.ReadExactly(new(&data, sizeof(T)));
        return data;
    }

    public static void ReadExactly(this BinaryReader reader, Span<byte> buffer)
    {
        reader.BaseStream.ReadExactly(buffer);
    }
}