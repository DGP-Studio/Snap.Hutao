// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class RandomAccessRead
{
    public static async ValueTask ExactlyAsync(SafeFileHandle handle, Memory<byte> buffer, long fileOffset, CancellationToken token)
    {
        int bytesRead = 0;
        while (true)
        {
            int read = await RandomAccess.ReadAsync(handle, buffer[bytesRead..], fileOffset + bytesRead, token).ConfigureAwait(false);

            if (read is 0)
            {
                throw new IOException("End of file has been reached");
            }

            bytesRead += read;

            if (bytesRead == buffer.Length)
            {
                break;
            }
        }
    }
}