// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO.Compression.Zstandard;

internal static class Zstandard
{
    public static void Decompress(Stream inputStream, Stream outputStream)
    {
        ArgumentNullException.ThrowIfNull(inputStream);
        ArgumentNullException.ThrowIfNull(outputStream);

        using (ZstandardDecompressionStream decompressionStream = new(inputStream))
        {
            decompressionStream.CopyTo(outputStream);
        }
    }

    public static async ValueTask DecompressAsync(Stream inputStream, Stream outputStream, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(inputStream);
        ArgumentNullException.ThrowIfNull(outputStream);

        using (ZstandardDecompressionStream decompressionStream = new(inputStream))
        {
            await decompressionStream.CopyToAsync(outputStream, token).ConfigureAwait(false);
        }
    }
}