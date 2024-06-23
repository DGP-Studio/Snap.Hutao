// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO;

internal sealed class StreamCopyStatus
{
    public StreamCopyStatus(long bytesCopied, long totalBytes)
    {
        BytesCopied = bytesCopied;
        TotalBytes = totalBytes;
    }

    public long BytesCopied { get; }

    public long TotalBytes { get; }
}