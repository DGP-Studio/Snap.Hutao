// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class RandomAccessRead
{
    public static bool Exactly(SafeFileHandle handle, Span<byte> buffer, long fileOffset)
    {
        int bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            int read = RandomAccess.Read(handle, buffer[bytesRead..], fileOffset + bytesRead);
            bytesRead += read;

            if (read is 0)
            {
                return false;
            }
        }

        return bytesRead == buffer.Length;
    }

    public static async ValueTask ExactlyAsync(SafeFileHandle handle, Memory<byte> buffer, long fileOffset, CancellationToken token)
    {
        int bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            int read = await RandomAccess.ReadAsync(handle, buffer[bytesRead..], fileOffset + bytesRead, token).ConfigureAwait(false);
            bytesRead += read;

            if (read is 0)
            {
                throw new IOException("End of file has been reached");
            }
        }
    }
}