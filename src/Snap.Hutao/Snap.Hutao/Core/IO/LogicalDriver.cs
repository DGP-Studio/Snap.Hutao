// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

internal static class LogicalDriver
{
    public static long GetAvailableFreeSpace(string path)
    {
        string? root = Path.GetPathRoot(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(root, "The path does not contain a root.");
        return new DriveInfo(root).AvailableFreeSpace;
    }
}