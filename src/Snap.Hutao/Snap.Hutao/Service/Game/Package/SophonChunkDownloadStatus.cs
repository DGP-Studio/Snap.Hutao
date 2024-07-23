// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

internal sealed class SophonChunkDownloadStatus
{
    public SophonChunkDownloadStatus(long bytesRead, bool finished)
    {
        BytesRead = bytesRead;
        Finished = finished;
    }

    public long BytesRead { get; }

    public bool Finished { get; }
}
