// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;

namespace Snap.Hutao.Service;

internal sealed class UpdateStatus
{
    public UpdateStatus(string version, long bytesRead, long totalBytes)
    {
        Version = version;
        BytesRead = bytesRead;
        TotalBytes = totalBytes;
        ProgressDescription = $"{Converters.ToFileSizeString(bytesRead)}/{Converters.ToFileSizeString(totalBytes)}";
    }

    public string? Version { get; set; }

    public long BytesRead { get; set; }

    public long TotalBytes { get; set; }

    public string ProgressDescription { get; }
}