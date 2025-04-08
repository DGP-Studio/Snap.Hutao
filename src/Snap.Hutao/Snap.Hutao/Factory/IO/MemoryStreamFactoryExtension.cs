// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Factory.IO;

internal static class MemoryStreamFactoryExtension
{
    public static async ValueTask<MemoryStream> GetStreamAsync(this IMemoryStreamFactory memoryStreamFactory, Stream copyFrom, bool resetSourcePosition = false)
    {
        MemoryStream targetStream = memoryStreamFactory.GetStream();
        await copyFrom.CopyToAsync(targetStream).ConfigureAwait(false);
        targetStream.Position = 0;
        if (resetSourcePosition && copyFrom.CanSeek)
        {
            copyFrom.Position = 0;
        }

        return targetStream;
    }
}