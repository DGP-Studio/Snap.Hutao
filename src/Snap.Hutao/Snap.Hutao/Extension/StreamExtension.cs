// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.IO;
using System.Buffers;
using System.IO;

namespace Snap.Hutao.Extension;

internal static class StreamExtension
{
    public static async ValueTask<Stream> CloneSegmentAsync(this Stream inputStream, long startPosition, long length, IMemoryStreamFactory memoryStreamFactory)
    {
        MemoryStream clonedStream = memoryStreamFactory.GetStream();
        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(81920))
        {
            Memory<byte> buffer = memoryOwner.Memory;
            inputStream.Position = startPosition;
            long bytesToCopy = length;
            while (bytesToCopy > 0)
            {
                int bytesRead = await inputStream.ReadAsync(buffer[..(int)Math.Min(buffer.Length, bytesToCopy)]).ConfigureAwait(false);
                if (bytesRead <= 0)
                {
                    break;
                }

                await clonedStream.WriteAsync(buffer[..bytesRead]).ConfigureAwait(false);
                bytesToCopy -= bytesRead;
            }

            clonedStream.Position = 0;
            return clonedStream;
        }
    }

    public static async ValueTask ReadExactAsync(this Stream stream, Memory<byte> buffer)
    {
        int bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer[bytesRead..]).ConfigureAwait(false);
            if (read == 0)
            {
                throw new EndOfStreamException();
            }

            bytesRead += read;
        }
    }
}
