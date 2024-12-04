// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO;

internal sealed class StreamCopyStatus
{
    public StreamCopyStatus(long bytesReadSinceLastReport, long bytesReadSinceCopyStart, long totalBytes)
    {
        BytesReadSinceLastReport = bytesReadSinceLastReport;
        BytesReadSinceCopyStart = bytesReadSinceCopyStart;
        TotalBytes = totalBytes;
    }

    public long BytesReadSinceLastReport { get; }

    public long BytesReadSinceCopyStart { get; }

    public long TotalBytes { get; }
}