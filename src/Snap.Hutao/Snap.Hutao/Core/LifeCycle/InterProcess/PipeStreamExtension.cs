// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.IO.Hashing;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

internal static class PipeStreamExtension
{
    public static unsafe byte[] GetValidatedContent(this PipeStream stream, PipePacketHeader* header)
    {
        byte[] content = new byte[header->ContentLength];
        stream.ReadAtLeast(content, header->ContentLength, false);
        HutaoException.ThrowIf(XxHash64.HashToUInt64(content) != header->Checksum, "PipePacket Content Hash incorrect");
        return content;
    }

    public static unsafe PipePacketHeader ReadPacket<TData>(this PipeStream stream, out TData? data)
        where TData : class
    {
        data = default;

        Span<byte> headerSpan = stackalloc byte[sizeof(PipePacketHeader)];
        stream.ReadExactly(headerSpan);
        fixed (byte* pHeader = headerSpan)
        {
            PipePacketHeader* header = (PipePacketHeader*)pHeader;
            if (header->ContentType is PipePacketContentType.Json)
            {
                ReadOnlySpan<byte> content = stream.GetValidatedContent(header);
                data = JsonSerializer.Deserialize<TData>(content);
            }

            return *header;
        }
    }

    public static unsafe void WritePacket(this PipeStream stream, PipePacketHeader* header, byte[] content)
    {
        header->ContentLength = content.Length;
        header->Checksum = XxHash64.HashToUInt64(content);

        stream.Write(new(header, sizeof(PipePacketHeader)));
        stream.Write(content);
    }
}
