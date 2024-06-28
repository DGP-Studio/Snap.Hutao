// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.Buffers;
using System.IO.Hashing;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

internal static class PipeStreamExtension
{
    public static TData? ReadJsonContent<TData>(this PipeStream stream, ref readonly PipePacketHeader header)
    {
        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(header.ContentLength))
        {
            Span<byte> content = memoryOwner.Memory.Span[..header.ContentLength];
            stream.ReadExactly(content);

            HutaoException.ThrowIf(XxHash64.HashToUInt64(content) != header.Checksum, "PipePacket Content Hash incorrect");
            return JsonSerializer.Deserialize<TData>(content);
        }
    }

    public static unsafe void ReadPacket<TData>(this PipeStream stream, out PipePacketHeader header, out TData? data)
        where TData : class
    {
        data = default;

        stream.ReadPacket(out header);
        if (header.ContentType is PipePacketContentType.Json)
        {
            data = stream.ReadJsonContent<TData>(in header);
        }
    }

    public static unsafe void ReadPacket(this PipeStream stream, out PipePacketHeader header)
    {
        fixed (PipePacketHeader* pHeader = &header)
        {
            stream.ReadExactly(new(pHeader, sizeof(PipePacketHeader)));
        }
    }

    public static unsafe void WritePacketWithJsonContent<TData>(this PipeStream stream, byte version, PipePacketType type, PipePacketCommand command, TData data)
    {
        PipePacketHeader header = default;
        header.Version = version;
        header.Type = type;
        header.Command = command;
        header.ContentType = PipePacketContentType.Json;

        stream.WritePacket(ref header, JsonSerializer.SerializeToUtf8Bytes(data));
    }

    public static unsafe void WritePacket(this PipeStream stream, ref PipePacketHeader header, byte[] content)
    {
        header.ContentLength = content.Length;
        header.Checksum = XxHash64.HashToUInt64(content);

        stream.WritePacket(in header);
        stream.Write(content);
    }

    public static unsafe void WritePacket(this PipeStream stream, byte version, PipePacketType type, PipePacketCommand command)
    {
        PipePacketHeader header = default;
        header.Version = version;
        header.Type = type;
        header.Command = command;

        stream.WritePacket(in header);
    }

    public static unsafe void WritePacket(this PipeStream stream, ref readonly PipePacketHeader header)
    {
        fixed (PipePacketHeader* pHeader = &header)
        {
            stream.Write(new(pHeader, sizeof(PipePacketHeader)));
        }
    }
}
